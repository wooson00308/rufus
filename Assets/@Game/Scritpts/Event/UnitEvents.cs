using UnityEngine;

public enum UnitEvents
{
    Spawn = 10001,
    Attack,
    Stun,
    Hit,
    Revive,
    Death,
    Kill
}

public class UnitEventArgs
{
    public Unit publisher;
}


public class UnitHitEventArgs : UnitEventArgs
{
    public Unit attacker;
}

public class UnitAttackEventArgs : UnitEventArgs
{
    public Unit target;
}
