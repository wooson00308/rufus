using UnityEngine;

public abstract class FxData : Data
{
    public abstract void OnEvent(Unit owner, Unit target);
}
