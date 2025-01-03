using UnityEngine;

public class HitState : StateBase
{
    public override void OnEnter(Unit unit)
    {
        unit.CrossFade("Hit");
    }

    public override void OnExit(Unit unit)
    {

    }

    public override void OnUpdate(Unit unit)
    {

    }
}
