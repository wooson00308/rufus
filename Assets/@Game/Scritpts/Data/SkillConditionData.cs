using UnityEngine;
public abstract class SkillConditionData : Data
{
    public virtual int EventId { get; set; }

    public virtual bool IsSatisfied(Skill skill, object gameEvent)
    {
        if (gameEvent is not UnitEventArgs args) return false;
        if (args is SkillEventArgs skillArgs && 
            skillArgs.skillId != skill.Data.Id) return false;
        
        return true;
    }

    public void TryUseSkill(Skill skill, object gameEvent)
    {
        if (!IsSatisfied(skill, gameEvent)) return;

        skill.UseSkill();
    }
}
