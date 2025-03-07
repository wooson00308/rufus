using UnityEngine;

[CreateAssetMenu(fileName = "EngageStartFxData", menuName = "Scriptable Objects/EngageStartFxData")]
public class EngageStartFxData : FxEventData
{
    public override void OnEventToTarget(Unit owner, Unit target)
    {
        ProcessSystem.Instance.OnNextProcess<EngageProcess>();
    }
}
