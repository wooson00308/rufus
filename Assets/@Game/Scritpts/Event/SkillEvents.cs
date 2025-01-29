using UnityEngine;

public enum SkillEvents
{
    UseSkill = 300,
    ApplySkill,
    RemoveSkill 
}

public class SkillEventArgs: UnitEventArgs
{
    public SkillData data;
}
