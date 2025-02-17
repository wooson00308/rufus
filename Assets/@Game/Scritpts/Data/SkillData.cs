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
            Debug.LogError($"{Id}의 해당 레벨 스킬 데이터가 존재하지 않습니다.");
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

    public string CastResult(int maxLevel)
    {
        int level = 1;
        string cast = "";
        foreach(var levelData in LevelDatas)
        {
            cast += levelData.Cast;

            if(++level > maxLevel)
            {
                break;
            }

            cast += " ";
        }

        return cast;
    }

    [field: SerializeField] public Skill Prefab { get; private set; }
    [field: SerializeField] public Color Color { get; private set; }
    [field: SerializeField] public List<SkillLevelData> LevelDatas { get; private set; }
}

[Serializable]
public class SkillLevelData
{
    [field: SerializeField] public int ADRatio { get; private set; }
    [field: SerializeField] public int APRatio { get; private set; }
    [field: SerializeField] public int ManaCost { get; private set; }
    [field: SerializeField] public int FailedManaCost { get; private set; }
    [field: SerializeField] public string DisplayName { get; private set; }
    [field: SerializeField] public string Description { get; private set; }
    [field: SerializeField] public float Duration { get; private set; }
    [field: SerializeField] public float Cooltime { get; private set; }
    [field: SerializeField] public string Cast { get; private set; }
    [field: SerializeField] public Color Color { get; private set; }

    [field: SerializeField] public List<ConditionData> Conditions { get; private set; }
    [field: SerializeField] public List<SkillFxEventData> ApplyFxDatas { get; private set; }
    [field: SerializeField] public List<SkillFxEventData> UseSkillFxDatas { get; private set; }
    [field: SerializeField] public List<SkillFxEventData> RemoveFxDatas { get; private set; }
}
