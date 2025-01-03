using UnityEngine;

public abstract class Data : ScriptableObject
{
    [field: SerializeField] public int Id { get; private set; }
    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField] public string DisplayName { get; private set; }
    [field: SerializeField] public string Description { get; private set; }
}
