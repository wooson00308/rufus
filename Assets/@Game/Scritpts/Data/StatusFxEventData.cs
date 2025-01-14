using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StatusFxEventData", menuName = "Scriptable Objects/StatusFxEventData")]
public class StatusFxEventData : FxEventData
{
    [field: SerializeField] public List<StatusFxData> StatusFxDatas { get; private set; }

    public override void OnEventToTarget(Unit owner, Unit target)
    {
        foreach (var statusFx in StatusFxDatas)
        {
            target.ApplyStatusFx(statusFx);
        }
    }
}
