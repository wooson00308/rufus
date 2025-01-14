using UnityEngine;

public abstract class FxEventData : Data
{
    public abstract void OnEvent(Unit owner, Unit target, object args = null);
}
