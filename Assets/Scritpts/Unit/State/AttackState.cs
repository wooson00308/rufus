using UnityEngine;

public class AttackState : StateBase
{
    public override void OnEnter(Unit unit)
    {
        unit.CrossFade("Attack");
    }

    public override void OnExit(Unit unit)
    {

    }

    public override void OnUpdate(Unit unit)
    {

    }
}
