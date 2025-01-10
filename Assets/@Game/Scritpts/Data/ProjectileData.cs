using System.Collections.Generic;
using UnityEngine;

public abstract class ProjectileData : Data
{
    [field: SerializeField] public bool HasHoming { get; private set; }
    [field: SerializeField] public Projectile Prefab { get; private set; }
    [field: SerializeField] public FloatStat MoveSpeed { get; private set; }
    [field: SerializeField] public FloatStat Duration { get; private set; }
    [field: SerializeField] public List<StatusFxData> StatusFxDatas { get; private set; } 

    public abstract void OnTriggerEnterEvent(Unit owner, Unit target);
    public abstract void OnTriggerStayEvent(Unit owner, Unit target);
    public abstract void OnTriggerExitEvent(Unit owner, Unit target);
}
