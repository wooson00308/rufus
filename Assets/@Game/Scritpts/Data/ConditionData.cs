using UnityEngine;

public abstract class ConditionData : Data
{
    public virtual int EventId { get; set; }

    public virtual bool IsSatisfied(Skill skill, GameEvent gameEvent)
    {
        UnitEventArgs args = (UnitEventArgs)gameEvent.args;
        if (!skill.Owner.EqualsUnit(args.publisher)) return false;
        return true;
    }

    public void TryUseSkill(Skill skill, GameEvent gameEvent)
    {
        if (!IsSatisfied(skill, gameEvent)) return;

        skill.UseSkill();
    }
}
