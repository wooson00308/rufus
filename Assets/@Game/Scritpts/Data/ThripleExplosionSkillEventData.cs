using UnityEngine;

[CreateAssetMenu(fileName = "ThripleExplosionSkillEventData", menuName = "Scriptable Objects/ThripleExplosionSkillEventData")]
public class ThripleExplosionSkillEventData : SkillFxEventData
{
    public int FirstExplosionRatio;
    public int SecondExplosionRatio;
    public int ThirdExplosionRatio;

    public override void OnSkillEvent(Unit owner, Skill skill)
    {
        // 세부 구현
        int adRatio = skill.CurrentLevelData.ADRatio;
        int apRatio = skill.CurrentLevelData.APRatio;

        Debug.Log($"First: {FirstExplosionRatio + adRatio + apRatio}"); 
        Debug.Log($"Second: {SecondExplosionRatio + adRatio + apRatio}"); 
        Debug.Log($"Third: {ThirdExplosionRatio + adRatio + apRatio}"); 
    }
}
