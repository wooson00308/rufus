using UnityEngine;

[CreateAssetMenu(fileName = "ReadyStartFxData", menuName = "Scriptable Objects/ReadyStartFxData")]
public class ReadyStartFxData : FxEventData
{
    public override void OnEventToTarget(Unit owner, Unit target)
    {
        ProcessSystem.Instance.OnNextProcess<ReadyProcess>();
    }
}
