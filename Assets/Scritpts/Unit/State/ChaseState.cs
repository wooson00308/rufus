using UnityEngine;

public class ChaseState : StateBase
{
    public override void OnEnter(Unit unit)
    {
        unit.CrossFade("Chase");
    }

    public override void OnExit(Unit unit)
    {

    }

    public override void OnUpdate(Unit unit)
    {

    }
}
