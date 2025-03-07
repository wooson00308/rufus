using UnityEngine;

[CreateAssetMenu(fileName = "InteractiveFxEventData", menuName = "Scriptable Objects/InteractiveFxEventData")]
public class InteractiveFxEventData : FxEventData
{
    public InteractiveEvents _eventType;

    public override void OnEventToTarget(Unit owner, Unit target)
    {
        GameEventSystem.Instance.Publish((int)_eventType);
    }
}
