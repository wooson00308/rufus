using UnityEngine;
public abstract class SkillConditionData : ConditionData
{
    public override bool IsSatisfied(Skill skill, object gameEvent)
    {
        if (gameEvent is not UnitEventArgs args) return false;
        if (args is SkillEventArgs skillArgs && 
            skillArgs.skillId != skill.Data.Id) return false;
        if (!args.publisher.EqualsUnit(skill.Owner)) return false;
        
        return true;
    }
}
