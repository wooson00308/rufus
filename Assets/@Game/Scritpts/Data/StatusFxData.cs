using UnityEngine;

public abstract class StatusFxData : Data
{
    [field: SerializeField] public Item Prefab { get; private set; }
    [field: SerializeField] public float Duration { get; private set; }

    public abstract void OnApply(Unit unit);
    public abstract void OnUpdate(Unit unit);
    public abstract void OnRemove(Unit unit);
}
