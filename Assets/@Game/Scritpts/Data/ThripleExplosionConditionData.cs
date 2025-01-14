using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ThripleExplosionConditionData", menuName = "Scriptable Objects/ThripleExplosionConditionData")]
public class ThripleExplosionConditionData : ConditionData
{
    [field: SerializeField] public int NeedMp { get; private set; }
    [field: SerializeField] public UnitEvents UnitEvent { get; private set; }
    public override int EventId { get { return (int)UnitEvent; } }

    public override bool IsSatisfied(Skill skill, GameEvent gameEvent)
    {
        if (!base.IsSatisfied(skill, gameEvent)) return false;

        var owner = skill.Owner;
        if (owner.Status.Mana.Value < NeedMp) return false;

        owner.Status.Mana.Update(Id.ToString(), -NeedMp);

        return true;
    }
}
