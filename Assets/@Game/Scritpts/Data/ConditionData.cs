using UnityEngine;

public abstract class ConditionData : Data
{
    public abstract bool IsSatisfied(Unit owner, object args = null);
}
