using UnityEngine;

public class IdleState : StateBase
{
    public override void OnEnter(Unit unit)
    {
        unit.CrossFade("Idle");
    }

    public override void OnExit(Unit unit)
    {
        
    }

    public override void OnUpdate(Unit unit)
    {
        if(unit.Target != null)
        {
            _fsm.TransitionTo<ChaseState>();
        }
    }
}
