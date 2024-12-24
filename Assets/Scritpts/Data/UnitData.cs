using UnityEngine;

[CreateAssetMenu(fileName = "UnitData", menuName = "Scriptable Objects/UnitData")]
public class UnitData : Data, IStats
{
    [field: SerializeField] public Sprite Icon { get; private set; }
    [field: SerializeField] public Unit Prefab { get; private set; }
    [field:SerializeField] public IntStat Health { get; private set;}
    [field: SerializeField] public IntStat Armor { get; private set; }
    [field: SerializeField] public IntStat AttackDamage { get; private set; }
    [field: SerializeField] public FloatStat AttackSpeed { get; private set; }
    [field: SerializeField] public FloatStat MoveSpeed { get; private set; }
}
