using UnityEngine;

[CreateAssetMenu(fileName = "MultiShotSkillConditionData", menuName = "Scriptable Objects/MultiShotSkillConditionData")]
public class MultiShotSkillConditionData : SkillConditionData
{
    [field: SerializeField] public int NeedMp { get; private set; }
    public override int EventId { get { return (int)UnitEvents.Death; } }

    /// <summary>
    /// 유닛이 죽는 이벤트를 감지해, 어태커가 본인이면 실행 
    /// </summary>
    /// <param name="skill"></param>
    /// <param name="gameEvent"></param>
    /// <returns></returns>
    public override bool IsSatisfied(Skill skill, object gameEvent)
    {
        if (!base.IsSatisfied(skill, gameEvent)) return false;

        if (gameEvent is not UnitEventWithAttackerArgs args) return false;
        if (!skill.Owner.EqualsUnit(args.attacker)) return false;

        var owner = skill.Owner;
        if (owner == null) return false;
        if (!owner.IsActive) return false;
        if (owner.Status.Mana.Value < NeedMp) return false;

        owner.Status.Mana.Update(Id.ToString(), -NeedMp);

        return true;
    }
}
