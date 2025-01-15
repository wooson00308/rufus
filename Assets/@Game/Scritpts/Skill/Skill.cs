using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 유닛이 보유하고 있는 스킬 정보
/// </summary>
public class Skill : MonoBehaviour
{
    private Unit _owner;
    private SkillData _data;
    private SkillLevelData _currentLevelData;
    private readonly Dictionary<SkillConditionData, Action<object>> _conditionActions = new();

    private int _level = 1;
    private bool _isInitialized;

    // === 추가: 쿨타임 / 듀레이션 관련 필드 ===
    private bool _isCoolingDown;
    private float _cooltimeRemain;    // 남은 쿨타임
    private bool _isDurationActive;
    private float _durationRemain;    // 남은 지속시간

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

        // 스킬이 활성화될 때 적용되는 이펙트 (버프 등)
        foreach (var fxEventData in _currentLevelData.ApplyFxDatas)
        {
            fxEventData.OnSkillEvent(_owner, this);
        }

        _isInitialized = true;
    }

    /// <summary>
    /// OnDisable 될 때 (오브젝트가 비활성화될 때) 처리
    /// </summary>
    public void OnDisable()
    {
        // 스킬이 비활성화될 때 제거해야 할 이펙트가 있다면 여기서 처리
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

        Action<object> OnEvent(SkillConditionData condition)
        {
            // 미리 delegate(람다) 생성 후 저장
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
    /// (직접 호출하거나 Condition이 만족했을 때) 스킬 사용
    /// </summary>
    public void UseSkill()
    {
        // 이미 쿨타임 중이면 스킬 사용 불가
        if (_isCoolingDown)
        {
            Debug.Log($"[{name}] 스킬이 쿨타임입니다. 남은 쿨타임: {_cooltimeRemain:F2}s");
            return;
        }

        _isCoolingDown = true;

        // 스킬 사용 FX 처리
        foreach (var fxEventData in _currentLevelData.UseSkillFxDatas)
        {
            fxEventData.OnEvent(_owner);
        }

        // === 추가: 쿨타임 설정 ===
        _cooltimeRemain = _currentLevelData.Cooltime;

        // === 추가: 듀레이션 설정(지속효과가 있는 스킬이라면) ===
        if (_currentLevelData.Duration > 0f)
        {
            _isDurationActive = true;
            _durationRemain = _currentLevelData.Duration;
        }
        else
        {
            _isDurationActive = false;
        }

        Debug.Log($"[{name}] 스킬 사용! (쿨타임 {_cooltimeRemain:F2}s, 지속시간 {_durationRemain:F2}s)");
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

    // === 추가: 쿨타임 / 듀레이션 갱신 로직 ===
    public void Update()
    {
        // 1) 쿨타임이 돌고 있다면
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

                // 이곳에서 지속 효과가 끝났을 때의 이펙트나 처리 호출
                Debug.Log($"[{name}] 스킬 효과(듀레이션)가 종료되었습니다.");

                // RemoveFxDatas 같은 걸 여기서 호출할 수도 있음
                // (만약 "스킬 효과가 끝나는 시점"에만 발동하는 로직이 필요하다면)
            }
        }
    }
}
