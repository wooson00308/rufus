using UnityEngine;

public class PlayerMoveState : StateBase
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
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            unit.UseSkill();
        }

        var moveVectorX = Input.GetAxisRaw("Horizontal");
        var moveVectorY = Input.GetAxisRaw("Vertical");

        var moveVector = new Vector2(moveVectorX, moveVectorY).normalized;

        if(moveVector == Vector2.zero)
        {
            _fsm.TransitionTo<PlayerIdleState>();
            return;
        }

        unit.Move(moveVector);

        if (Input.GetKey(KeyCode.Mouse0) && _fsm.CurrentStateName != "PlayerAttackState")
        {
            _fsm.TransitionTo<PlayerAttackState>();
        }
    }
}
