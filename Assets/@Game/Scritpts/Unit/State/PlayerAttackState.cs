using UnityEngine;

public class PlayerAttackState : StateBase
{
    [SerializeField] private int _animationSize = 1;
    private string _currentState;
    private int _currentStateIndex;

    private float _currentAttackStateCheckDelay = 0;

    public override void OnEnter(Unit unit)
    {
        _currentStateIndex = Random.Range(0, _animationSize);
        _currentState = $"Attack {_currentStateIndex + 1}";
        unit.CrossFade(_currentState, 0f);
        unit.Stop();
    }

    public override void OnExit(Unit unit)
    {
        _currentAttackStateCheckDelay = 0;
    }

    public override void OnUpdate(Unit unit)
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            unit.UseSkill();
        }

        if (_currentAttackStateCheckDelay <= 0.5f)
        {
            _currentAttackStateCheckDelay += Time.deltaTime;
            return;
        }

        Vector3 direction;

        if (unit.Target == null)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0;

            direction = (mousePosition - unit.transform.position).normalized;
        }
        else
        {
            direction = unit.Target.transform.position - unit.transform.position;
        }

        unit.Rotation(direction);

        if (unit.GetAttackState() == 0)
        {
            _fsm.TransitionTo<PlayerMoveState>();
        }
    }
}
