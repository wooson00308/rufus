using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "Scriptable Objects/ItemData")]
public class ItemData : Data, IStats
{
    [field: SerializeField] public Item Prefab { get; private set; }
    [field: SerializeField] public EquipType EquipType { get; private set; }
    [field: SerializeField] public IntStat Health { get; private set; }
    [field: SerializeField] public IntStat Mana { get; private set; }
    [field: SerializeField] public IntStat Armor { get; private set; }
    [field: SerializeField] public IntStat AttackDamage { get; private set; }
    [field: SerializeField] public FloatStat AttackSpeed { get; private set; }
    [field: SerializeField] public FloatStat AttackRange { get; private set; }
    [field: SerializeField] public FloatStat MoveSpeed { get; private set; }
    [field: SerializeField] public FloatStat AoERadius { get; private set; }
}

public enum EquipType
{
    None,
    Helmet,
    Armor,
    Weapon
}
