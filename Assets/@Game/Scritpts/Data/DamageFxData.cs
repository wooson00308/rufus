using UnityEngine;

[CreateAssetMenu(fileName = "DamageFxData", menuName = "Scriptable Objects/DamageFxData")]
public class DamageFxData : FxEventData
{
    public int DamageCoefficient;
    public override void OnEvent(Unit owner, Unit target)
    {
        float damageCoefficient = DamageCoefficient / 100f; // 데미지 계수를 계산 (100 기준)
        int totalDamage = (int)(owner.Status.AttackDamage.Value * damageCoefficient); // 실제 데미지 계산
        target.OnHit(totalDamage, owner);
    }
}
