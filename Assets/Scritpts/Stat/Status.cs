using UnityEngine;

public class Status : MonoBehaviour, IStats, IStatSettable
{
    [HideInInspector] public IntStat Health { get; private set; }
    [HideInInspector] public IntStat Armor { get; private set; }
    [HideInInspector] public IntStat AttackDamage { get; private set; }
    [HideInInspector] public FloatStat AttackSpeed { get; private set; }
    [HideInInspector] public FloatStat AttackRange { get; private set; }
    [HideInInspector] public FloatStat MoveSpeed { get; private set; }

    public bool IsStun { get; private set; }
    public bool IsDeath { get; private set; }

    public void IntializeStats(IStats stats)
    {
        Health = new IntStat(stats.Health.Value);
        Armor = new IntStat(stats.Armor.Value);
        AttackDamage = new IntStat(stats.AttackDamage.Value);
        AttackSpeed = new FloatStat(stats.AttackSpeed.Value);
        AttackRange = new FloatStat(stats.AttackRange.Value);
        MoveSpeed = new FloatStat(stats.MoveSpeed.Value);
    }

    public void ResetStats(string key)
    {
        Health.Reset(key);
        Armor.Reset(key);
        AttackDamage.Reset(key);
        AttackSpeed.Reset(key);
        AttackRange.Reset(key);
        MoveSpeed.Reset(key);
    }

    public void UpdateStats(string key, IStats stats)
    {
        Health.Update(key, stats.Health.Value);
        Armor.Update(key, stats.Armor.Value);
        AttackDamage.Update(key, stats.AttackDamage.Value);
        AttackSpeed.Update(key, stats.MoveSpeed.Value);
        AttackRange.Update(key, stats.MoveSpeed.Value);
        MoveSpeed.Update(key, stats.MoveSpeed.Value);
    }

    public void OnStun(Unit attacker)
    {
        if (IsStun) return;
        if (IsDeath) return;

        IsStun = true;
    }

    public void OnHit(int damage, Unit attacker)
    {
        if (IsStun) return;
        if (IsDeath) return;

        Health.Update(Unit.ENGAGE_STATS_KEY, -damage);

        if(Health.Value <= 0)
        {
            OnDeath(attacker);
        }
    }

    public void OnDeath(Unit attacker)
    {
        if (IsDeath) return;
        IsDeath = true;
    }
}
