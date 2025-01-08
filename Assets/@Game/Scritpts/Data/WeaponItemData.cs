using UnityEngine;

[CreateAssetMenu(fileName = "WeaponItemData", menuName = "Scriptable Objects/WeaponItemData")]
public class WeaponItemData : ItemData
{
    [field: SerializeField] public ProjectileData ProjectileData { get; private set; }
}
