using UnityEngine;

public abstract class FxEventData : Data
{
    public virtual void OnEventToTarget(Unit owner, Unit target) { }
    public virtual void OnEvent(Unit owner, object args = null) { }
}
