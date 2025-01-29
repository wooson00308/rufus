using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 스킬을 관리하고 있는 스킬 클래스
/// </summary>
public class Skill : MonoBehaviour
{
    private Unit _owner;
    private SkillData _data;
    private SkillLevelData _currentLevelData;
    private readonly Dictionary<ConditionData, Action<object>> _conditionActions = new();

    private int _level = 1;
    private bool _isInitialized;

    // === 추가: 쿨타임 / 지속시간 관련 필드 ===
    private bool _isCoolingDown;
    private float _cooltimeRemain;    // 현재 쿨타임
    private bool _isDurationActive;
    private float _durationRemain;    // 현재 지속시간

    public Unit Owner => _owner;
    public SkillData Data => _data;
    public SkillLevelData CurrentLevelData => _currentLevelData;

    public int Level => _level;

    /// <summary>
    /// 스킬 초기화
    /// </summary>
    public void Initialized(Unit owner, SkillData data)
    {
        _owner = owner;
        _data = data;

        _level = 1;
        _currentLevelData = _data.GetSkillLevelData(_level);

        SubscribeConditionEvents();

        // 스킬이 활성화될 때 발생하는 이벤트 (버프 등)
        foreach (var fxEventData in _currentLevelData.ApplyFxDatas)
        {
            fxEventData.OnSkillEvent(_owner, this);
        }

        _isInitialized = true;
    }

    /// <summary>
    /// OnDisable 시 호출 (오브젝트가 비활성화될 때) 처리
    /// </summary>
    public void OnDisable()
    {
        // 스킬이 비활성화될 때 정리해야 할 이벤트가 있다면 여기서 처리
        foreach (var fxEventData in _currentLevelData.RemoveFxDatas)
        {
            fxEventData.OnSkillEvent(_owner, this);
        }

        _isInitialized = false;
        UnsubscribeConditionEvents();
    }

    private void SubscribeConditionEvents()
    {
        foreach (var condition in _currentLevelData.Conditions)
        {
            if (!_conditionActions.ContainsKey(condition))
            {
                Action<object> action = OnEvent(condition);
                _conditionActions.Add(condition, action);
            }
            GameEventSystem.Instance.Subscribe(condition.EventId, _conditionActions[condition]);
        }

        Action<object> OnEvent(ConditionData condition)
        {
            // 이벤트 delegate(델리게이트) 반환
            return (gameEvent) => { condition.TryUseSkill(this, gameEvent); };
        }
    }

    private void UnsubscribeConditionEvents()
    {
        foreach (var condition in _currentLevelData.Conditions)
        {
            if (_conditionActions.TryGetValue(condition, out var action))
            {
                GameEventSystem.Instance.Unsubscribe(condition.EventId, action);
                _conditionActions.Remove(condition);
            }
        }
    }

    /// <summary>
    /// (직접 호출하거나 Condition에 의해 트리거될 때) 스킬 발동
    /// </summary>
    public void UseSkill(int level = 0)
    {
        // 이미 쿨타임 중이면 스킬 사용 불가
        if (_isCoolingDown)
        {
            Debug.Log($"[{name}] 스킬이 쿨타임입니다. 남은 쿨타임: {_cooltimeRemain:F2}s");
            return;
        }

        _isCoolingDown = true;

        var levelData = level > 0 && level <= _data.LevelDatas.Count ?
            _data.GetSkillLevelData(level) :
            _currentLevelData;

        // 스킬 발동 FX 처리
        foreach (var fxEventData in levelData.UseSkillFxDatas)
        {
            fxEventData.OnEvent(_owner);
        }

        // === 추가: 쿨타임 설정 ===
        _cooltimeRemain = levelData.Cooltime;

        // === 추가: 지속시간 설정(버프 효과가 있는 스킬이라면) ===
        if (levelData.Duration > 0f)
        {
            _isDurationActive = true;
            _durationRemain = levelData.Duration;
        }
        else
        {
            _isDurationActive = false;
        }

        Debug.Log($"[{name}] 스킬 발동! (쿨타임 {_cooltimeRemain:F2}s, 지속시간 {_durationRemain:F2}s)");
    }

    /// <summary>
    /// 스킬 레벨 업
    /// </summary>
    public void LevelUp(int amount = 1)
    {
        if (!_isInitialized) return;
        if (_level >= _data.MaxLevel) return;

        int changeLevel = Math.Min(_level + amount, _data.MaxLevel);

        if (_level == changeLevel) return;
        _level = changeLevel;

        UnsubscribeConditionEvents();

        _currentLevelData = _data.GetSkillLevelData(_level);
        SubscribeConditionEvents();
    }

    // === 추가: 쿨타임 / 지속시간 관련 갱신 ===
    public void Update()
    {
        // 1) 쿨타임이 진행 중이라면
        if (_isCoolingDown)
        {
            _cooltimeRemain -= Time.deltaTime;
            if (_cooltimeRemain <= 0f)
            {
                _cooltimeRemain = 0f;
                _isCoolingDown = false;
                Debug.Log($"[{name}] 스킬 쿨타임이 종료되었습니다.");
            }
        }

        // 2) 지속 효과가 활성화되어 있다면
        if (_isDurationActive)
        {
            _durationRemain -= Time.deltaTime;
            if (_durationRemain <= 0f)
            {
                _durationRemain = 0f;
                _isDurationActive = false;

                // 여기에 지속 효과 종료 시 처리할 이벤트 실행
                Debug.Log($"[{name}] 스킬 효과(지속시간)가 종료되었습니다.");

                // RemoveFxDatas 관련 이벤트 여기서 실행 (버프 효과 종료 처리 등)
                foreach (var fxEventData in _currentLevelData.UseSkillFxDatas)
                {
                    fxEventData.OnEndEvent(_owner, this);
                }
            }
        }
    }
}
