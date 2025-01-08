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
        if (_currentAttackStateCheckDelay <= 0.5f)
        {
            _currentAttackStateCheckDelay += Time.deltaTime;
            return;
        }

        if (unit.Target == null)
        {
            _fsm.TransitionTo<PlayerMoveState>();
            return;
        }

        unit.Rotation(unit.Target.transform.position - unit.transform.position);

        if (unit.GetAttackState() == 0)
        {
            _fsm.TransitionTo<PlayerMoveState>();
        }
    }
}
