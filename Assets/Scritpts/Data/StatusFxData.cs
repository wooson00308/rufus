using UnityEngine;

[CreateAssetMenu(fileName = "StatusFxData", menuName = "Scriptable Objects/StatusFxData")]
public class StatusFxData : Data
{
    [field: SerializeField] public Item Prefab { get; private set; }
    [field: SerializeField] public float Duration { get; private set; }
}
