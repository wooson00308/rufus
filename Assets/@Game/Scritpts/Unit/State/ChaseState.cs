using UnityEngine;

public class ChaseState : StateBase
{
    public override void OnEnter(Unit unit)
    {
        if (unit.Status.MoveSpeed.Value < 5)
            unit.CrossFade("Walk");
        else
            unit.CrossFade("Run");
    }

    public override void OnExit(Unit unit)
    {

    }

    public override void OnUpdate(Unit unit)
    {
        if(unit.Target == null)
        {
            _fsm.TransitionTo<IdleState>();
            return;
        }

        unit.MoveFromTarget(unit.Target.transform);

        var distance = Vector2.Distance(unit.transform.position, unit.Target.transform.position);

        if(distance <= unit.Status.AttackRange.Value)
        {
            _fsm.TransitionTo<AttackState>();
        }
    }
}
