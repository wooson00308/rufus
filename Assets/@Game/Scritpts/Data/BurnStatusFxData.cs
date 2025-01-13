using UnityEngine;

[CreateAssetMenu(fileName = "BurnStatusFxData", menuName = "Scriptable Objects/BurnStatusFxData")]
public class BurnStatusFxData : StatusFxData
{
    public int Damage;

    public override void OnApply(Unit unit, Unit caster = null)
    {
        
    }

    public override void OnRemove(Unit unit, Unit caster = null)
    {
        
    }

    public override void OnUpdate(Unit unit, Unit caster = null)
    {
        unit.OnHit(Damage, caster);
    }
}
