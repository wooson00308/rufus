using UnityEngine;

public class CastingState : StateBase
{
    public override void OnEnter(Unit unit)
    {
        unit.CrossFade("Casting");
    }

    public override void OnExit(Unit unit)
    {

    }

    public override void OnUpdate(Unit unit)
    {

    }
}