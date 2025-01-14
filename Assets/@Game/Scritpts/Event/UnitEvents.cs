using UnityEngine;

public enum UnitEvents
{
    OnUseSkill = 10001,
    OnAttack,
    OnStun,
    OnHit,
    OnRevive,
    OnDeath,
    OnKill
}

public class UnitEventArgs
{
    public Unit publisher;
}
