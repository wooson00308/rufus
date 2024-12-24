public interface IState
{
    void OnEnter(Unit unit);
    void OnUpdate(Unit unit);
    void OnExit(Unit unit);
}
