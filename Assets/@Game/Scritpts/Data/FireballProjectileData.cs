using UnityEngine;

[CreateAssetMenu(fileName = "FireballProjectileData", menuName = "Scriptable Objects/FireballProjectileData")]
public class FireballProjectileData : ProjectileData
{
    [field: SerializeField] public IntStat DamageCoefficient { get; private set; }

    public override void OnTriggerEnterEvent(Unit owner, Unit target)
    {
        float damageCoefficient = DamageCoefficient.Value / 100f; // ������ ����� ��� (100 ����)
        int totalDamage = (int)(owner.Status.AttackDamage.Value * damageCoefficient); // ���� ������ ���
        target.OnHit(totalDamage, owner);

        foreach (var data in StatusFxDatas)
        {
            target.ApplyStatusFx(data);
        }
    }

    public override void OnTriggerExitEvent(Unit owner, Unit target)
    {

    }

    public override void OnTriggerStayEvent(Unit owner, Unit target)
    {

    }
}
