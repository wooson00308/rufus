using UnityEngine;

public class Status : MonoBehaviour, IStats, IStatSettable
{
    [field:SerializeField] public IntStat Health { get; private set; }
    [field: SerializeField] public IntStat Armor { get; private set; }
    [field: SerializeField] public IntStat AttackDamage { get; private set; }
    [field: SerializeField] public FloatStat AttackSpeed { get; private set; }
    [field: SerializeField] public FloatStat MoveSpeed { get; private set; }

    public void ResetStats(string key)
    {
        Health.Reset(key);
        Armor.Reset(key);
        AttackDamage.Reset(key);
        AttackSpeed.Reset(key);
        MoveSpeed.Reset(key);
    }

    public void UpdateStats(string key, IStats stats)
    {
        Health.Update(key, stats.Health.Value);
        Armor.Update(key, stats.Armor.Value);
        AttackDamage.Update(key, stats.AttackDamage.Value);
        AttackSpeed.Update(key, stats.MoveSpeed.Value);
        MoveSpeed.Update(key, stats.MoveSpeed.Value);
    }
}
