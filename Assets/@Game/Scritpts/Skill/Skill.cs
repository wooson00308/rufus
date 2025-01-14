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
    private readonly Dictionary<ConditionData, Action<GameEvent>> _conditionActions = new();

    private int _level;
    private bool _isInitialized;


    public Unit Owner => _owner;
    public SkillData Data => _data;
    public SkillLevelData CurrentLevelData => _currentLevelData;

    public int Level => _level;

    public void Initialized(Unit owner, SkillData data)
    {
        _owner = owner;

        _data = data;
        _currentLevelData = _data.GetSkillLevelData(_level);
        SubscribeConditionEvents();

        _isInitialized = true;
    }

    public void OnDisable()
    {
        _level = 1;
        _isInitialized = false;
        UnsubscribeConditionEvents();
    }

    private void SubscribeConditionEvents()
    {
        foreach(var condition in _currentLevelData.Conditions)
        {
            if (!_conditionActions.ContainsKey(condition))
            {
                Action<GameEvent> action = OnEvent(condition);
                _conditionActions.Add(condition, action);
            }

            GameEventSystem.Instance.Subscribe(condition.EventId, _conditionActions[condition]);
        }

        Action<GameEvent> OnEvent(ConditionData condition)
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
                // 구독 해제
                GameEventSystem.Instance.Unsubscribe(condition.EventId, action);
                _conditionActions.Remove(condition);
            }
        }
    }

    public void UseSkill()
    {
        foreach(var fxEventData in _currentLevelData.UseSkillFxDatas)
        {
            fxEventData.OnEvent(_owner);
        }
    } 

    public void LevelUp(int amount)
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
}
