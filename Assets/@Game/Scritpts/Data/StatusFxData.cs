using UnityEngine;

public abstract class StatusFxData : Data
{
    [field: SerializeField] public StatusFx Prefab { get; private set; }
    [field: SerializeField] public float Duration { get; private set; }

    public abstract void OnApply(Unit unit, Unit caster = null);
    public abstract void OnUpdate(Unit unit, Unit caster = null);
    public abstract void OnRemove(Unit unit, Unit caster = null);
}
