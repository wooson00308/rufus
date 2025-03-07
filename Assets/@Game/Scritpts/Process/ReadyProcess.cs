using UnityEngine;

public class ReadyProcess : Process
{
    public void OnEnable()
    {
        GameEventSystem.Instance.Subscribe((int)InteractiveEvents.Portal_Enter, NextProcess);
    }

    public void OnDisable()
    {
        GameEventSystem.Instance.Unsubscribe((int)InteractiveEvents.Portal_Enter, NextProcess);
    }

    private void NextProcess(object gameEvent)
    {
        _processSystem.OnNextProcess<LobbyProcess>();
    }
}
