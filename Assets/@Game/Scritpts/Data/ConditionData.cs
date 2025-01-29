using UnityEngine;

public abstract class ConditionData : Data
{
    public virtual int EventId { get; set; }

    public virtual bool IsSatisfied(Skill skill, object gameEvent)
    {
        return true;
    }

    public virtual void TryUseSkill(Skill skill, object gameEvent)
    {
        if (!IsSatisfied(skill, gameEvent)) return;

        skill.UseSkill();
    }
}
