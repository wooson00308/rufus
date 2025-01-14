using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillData", menuName = "Scriptable Objects/SkillData")]
public class SkillData : Data
{
    public int MaxLevel
    {
        get
        {
            if (LevelDatas == null) return 0;
            return LevelDatas.Count;
        }
    }

    public SkillLevelData GetSkillLevelData(int level)
    {
        int index = level - 1;
        SkillLevelData defaultData = LevelDatas[^1];

        if (defaultData == null)
        {
            string errorMessage = $"{Id}의 레벨 별 스킬 데이터가 존재하지 않습니다.";
            Debug.LogError(errorMessage);
            return null;
        }

        if (index >= MaxLevel || index < 0)
        {
            return defaultData;
        }

        return LevelDatas[index];
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
    [field: SerializeField] public List<SkillFxEventData> ApplyFxDatas { get; private set; }
    [field: SerializeField] public List<SkillFxEventData> UseSkillFxDatas { get; private set; }
    [field: SerializeField] public List<SkillFxEventData> RemoveFxDatas { get; private set; }
}
