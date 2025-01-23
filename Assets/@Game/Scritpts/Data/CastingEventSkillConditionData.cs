using UnityEngine;

[CreateAssetMenu(fileName = "CastingEventSkillConditionData", menuName = "Scriptable Objects/CastingEventSkillConditionData")]
public class CastingEventSkillConditionData : ConditionData
{
    public override int EventId => (int)SystemEvents.CastingEnd;

    public override bool IsSatisfied(Skill skill, object gameEvent)
    {
        if (!base.IsSatisfied(skill, gameEvent)) return false;
        if (gameEvent is not CastingEndEventArgs args) return false;
        if (!args.isSuccess) return false;
        if (skill.Data.Id != args.skillData.Id) return false;
        return true;
    }
}
