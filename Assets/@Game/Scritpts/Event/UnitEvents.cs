using UnityEngine;

public enum UnitEvents
{
    Spawn = 10001,
    Attack,
    Stun,
    Hit,
    Revive,
    Death,
}

public class UnitEventArgs
{
    public Unit publisher;
}

public class UnitEventWithAttackerArgs: UnitEventArgs
{
    public Unit attacker;
}
