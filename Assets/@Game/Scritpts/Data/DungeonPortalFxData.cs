using UnityEngine;

[CreateAssetMenu(fileName = "DungeonPortalFxData", menuName = "Scriptable Objects/DungeonPortalFxData")]
public class DungeonPortalFxData : FxEventData
{
    public bool IsExitEvent;

    public override void OnEventToTarget(Unit owner, Unit target)
    {
        var lobby = ProcessSystem.Instance.Get<LobbyProcess>();
        lobby.OnTriggeredPortal = !IsExitEvent;
    }
}
