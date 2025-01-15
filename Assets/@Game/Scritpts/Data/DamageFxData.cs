using UnityEngine;

[CreateAssetMenu(fileName = "DamageFxData", menuName = "Scriptable Objects/DamageFxData")]
public class DamageFxData : FxEventData
{
    public int DamageCoefficient;
    public override void OnEventToTarget(Unit owner, Unit target)
    {
        float damageCoefficient = DamageCoefficient / 100f; // ������ ����� ��� (100 ����)
        int totalDamage = (int)(owner.Status.AttackDamage.Value * damageCoefficient); // ���� ������ ���
        if (target == null) return;
        target.OnHit(totalDamage, owner);
    }
}
