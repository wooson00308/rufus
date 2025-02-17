using UnityEngine;

public class PlayerIdleState : StateBase
{
    public override void OnEnter(Unit unit)
    {
        unit.CrossFade("Idle");
        unit.Stop();
    }

    public override void OnExit(Unit unit)
    {
        
    }

    public override void OnUpdate(Unit unit)
    {
        var moveVectorX = Input.GetAxisRaw("Horizontal");
        var moveVectorY = Input.GetAxisRaw("Vertical");

        var moveVector = new Vector2(moveVectorX, moveVectorY).normalized;

        if(moveVector != Vector2.zero)
        {
            _fsm.TransitionTo<PlayerMoveState>();
        }

        if (Input.GetKey(KeyCode.J))
        {
            _fsm.TransitionTo<PlayerAttackState>();
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            unit.UseSkill();
        }
    }
}
