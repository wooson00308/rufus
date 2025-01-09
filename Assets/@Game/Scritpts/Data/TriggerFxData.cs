using UnityEngine;

[CreateAssetMenu(fileName = "TriggerFxData", menuName = "Scriptable Objects/TriggerFxData")]
public class TriggerFxData : Data
{
    [field: SerializeField] public int MaxMultiTriggerCount { get; private set; }
    [field: SerializeField] public float Durtaion { get; private set; }
    [field: SerializeField] public float EPS { get; private set; }
    [field: SerializeField] public bool OnEventFromSelf { get; private set; }
    [field: SerializeField] public bool OnEventFromOwnerProjectile { get; private set; }
}
