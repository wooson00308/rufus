using UnityEngine;

[CreateAssetMenu(fileName = "UnitEventSkillConditionData", menuName = "Scriptable Objects/UnitEventSkillConditionData")]
public class UnitEventSkillConditionData : SkillConditionData
{
    public UnitEvents UnitEvent;
    public override int EventId => (int)UnitEvent;

    /// <summary>
    /// 유닛이 특정 이벤트를 만족할 때, 스킬의 조건이 충족되는지 확인
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
