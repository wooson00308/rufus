using UnityEngine;

[CreateAssetMenu(fileName = "EngageStartFxData", menuName = "Scriptable Objects/EngageStartFxData")]
public class EngageStartFxData : FxEventData
{
    public bool IsExitEvent;
    public override void OnEventToTarget(Unit owner, Unit target)
    {
        ProcessSystem.Instance.Get<ReadyProcess>().OnTriggeredPortal = !IsExitEvent;
    }
}
