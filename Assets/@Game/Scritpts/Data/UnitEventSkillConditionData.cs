using UnityEngine;

[CreateAssetMenu(fileName = "UnitEventSkillConditionData", menuName = "Scriptable Objects/UnitEventSkillConditionData")]
public class UnitEventSkillConditionData : SkillConditionData
{
    public UnitEvents UnitEvent;
    public override int EventId { get { return (int)UnitEvent; } }

    /// <summary>
    /// 유닛이 죽는 이벤트를 감지해, 어태커가 본인이면 실행 
    /// </summary>
    /// <param name="skill"></param>
    /// <param name="gameEvent"></param>
    /// <returns></returns>
    public override bool IsSatisfied(Skill skill, object gameEvent)
    {
        if (!base.IsSatisfied(skill, gameEvent)) return false;

        var owner = skill.Owner;
        if (owner == null) return false;
        if (!owner.IsActive) return false;

        return true;
    }
}
