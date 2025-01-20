using UnityEngine;

public enum UnitEvents
{
    Spawn = 200,
    Attack,
    Stun,
    Casting,
    Hit,
    Revive,
    Death,
    Kill
}

public class UnitEventArgs
{
    public Unit publisher;
}

public class UnitCastingEventArgs : UnitEventArgs
{
    
}

public class UnitCastingEndEventArgs : UnitEventArgs
{
    public bool isSuccess;
}

public class UnitHitEventArgs : UnitEventArgs
{
    public Unit attacker;
}

public class UnitAttackEventArgs : UnitEventArgs
{
    public Unit target;
}
