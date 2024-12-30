using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "Scriptable Objects/ItemData")]
public class ItemData : Data, IStats
{
    public Item Prefab { get; private set; }
    public EquipType EquipType { get; private set; }
    public IntStat Health { get; private set; }
    public IntStat Armor { get; private set; }
    public IntStat AttackDamage { get; private set; }
    public FloatStat AttackSpeed { get; private set; }
    public FloatStat AttackRange { get; private set; }
    public FloatStat MoveSpeed { get; private set; }
}

public enum EquipType
{
    None,
    Helmet,
    Armor,
    Weapon
}
