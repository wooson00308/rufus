using System.Collections.Generic;
using UnityEngine;

public abstract class ProjectileData : Data
{
    [field: SerializeField] public bool HasHoming { get; private set; }
    [field: SerializeField] public Projectile Prefab { get; private set; }
    [field: SerializeField] public FloatStat MoveSpeed { get; private set; }
    [field: SerializeField] public FloatStat Duration { get; private set; }
 
    public virtual void OnTriggerEnterEvent(Unit owner, Unit target, object args = null) { }
    public virtual void OnTriggerStayEvent(Unit owner, Unit target, object args = null) { }
    public virtual void OnTriggerExitEvent(Unit owner, Unit target, object args = null) { }
}
