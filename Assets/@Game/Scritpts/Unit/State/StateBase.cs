using UnityEngine;

public abstract class StateBase : MonoBehaviour, IState
{
    protected FSM _fsm;

    public abstract void OnEnter(Unit unit);
    public abstract void OnExit(Unit unit);
    public abstract void OnUpdate(Unit unit);

    protected virtual void Awake()
    {
        _fsm = GetComponent<FSM>();
    }
}