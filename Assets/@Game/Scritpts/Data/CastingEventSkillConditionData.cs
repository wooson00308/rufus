using UnityEngine;

[CreateAssetMenu(fileName = "CastingEventSkillConditionData", menuName = "Scriptable Objects/CastingEventSkillConditionData")]
public class CastingEventSkillConditionData : ConditionData
{
    public override int EventId => (int)SystemEvents.CastingEnd;

    public override bool IsSatisfied(Skill skill, object gameEvent)
    {
        return true;
    }

    public override void TryUseSkill(Skill skill, object gameEvent)
    {
        if (!base.IsSatisfied(skill, gameEvent)) return;
        if (gameEvent is not CastingEndEventArgs args) return;
        if (!args.isSuccess) return;
        if (skill.Data.Id != args.skillData.Id) return;

        skill.UseSkill(args.succesLevel);
    }
}
