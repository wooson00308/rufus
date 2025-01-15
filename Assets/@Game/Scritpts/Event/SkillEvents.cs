using UnityEngine;

public enum SkillEvents
{
    UseSkill = 11001,
    ApplySkill,
    RemoveSkill 
}

public class SkillEventArgs: UnitEventArgs
{
    public int skillId;
}
