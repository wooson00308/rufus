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
        if (LevelDatas == null || LevelDatas.Count == 0)
        {
            Debug.LogError($"{Id}의 레벨 별 스킬 데이터가 비어있습니다.");
            return null;
        }

        int index = level - 1;
        SkillLevelData defaultData = level >= MaxLevel ? LevelDatas[^1] : LevelDatas[0];

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
    [field: SerializeField] public List<SkillConditionData> Conditions { get; private set; }
    [field: SerializeField] public List<SkillFxEventData> ApplyFxDatas { get; private set; }
    [field: SerializeField] public List<SkillFxEventData> UseSkillFxDatas { get; private set; }
    [field: SerializeField] public List<SkillFxEventData> RemoveFxDatas { get; private set; }
}
