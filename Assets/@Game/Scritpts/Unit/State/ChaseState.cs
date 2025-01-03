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
        if(unit.Target == null)
        {
            _fsm.TransitionTo<IdleState>();
        }

        unit.MoveFromTarget(unit.Target.transform);

        var distace = Vector2.Distance(unit.transform.position, unit.Target.transform.position);

        if(distace <= unit.Status.AttackRange.Value)
        {
            _fsm.TransitionTo<AttackState>();
        }
    }
}
