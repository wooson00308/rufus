using UnityEngine;

public class DeathState : StateBase
{
    public override void OnEnter(Unit unit)
    {
        unit.CrossFade("Death");
    }

    public override void OnExit(Unit unit)
    {

    }

    public override void OnUpdate(Unit unit)
    {

    }
}
