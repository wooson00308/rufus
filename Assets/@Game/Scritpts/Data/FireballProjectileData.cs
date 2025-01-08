using UnityEngine;

[CreateAssetMenu(fileName = "FireballProjectileData", menuName = "Scriptable Objects/FireballProjectileData")]
public class FireballProjectileData : ProjectileData
{
    public override void OnTriggerEnterEvent(Unit owner, Unit target)
    {
        target.OnHit(owner.Status.AttackDamage.Value, owner);
        foreach(var data in StatusFxDatas)
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
