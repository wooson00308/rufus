using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class SkillData : Data
{
    public int MaxLevel
    {
        get
        {
            if (LevelDatas == null) return 0;
            return LevelDatas.Count;
        }
    }

    [field: SerializeField] public Skill Prefab { get; private set; }
    [field: SerializeField] public List<SkillLevelData> LevelDatas { get; private set; }
    
}

[Serializable]
public class SkillLevelData
{
    [field: SerializeField] public int ADRatio { get; private set; }
    [field: SerializeField] public int APRatio { get; private set; }
    [field: SerializeField] public string Description { get; private set; }
    [field: SerializeField] public float Duration { get; private set; }
    [field: SerializeField] public float Cooltime { get; private set; }
    [field: SerializeField] public List<ConditionData> Conditions { get; private set; }
    [field: SerializeField] public List<FxEventData> ApplyFxDatas { get; private set; }
    [field: SerializeField] public List<FxEventData> UpdateFxDatas { get; private set; }
    [field: SerializeField] public List<FxEventData> RemoveFxDatas { get; private set; }
}
