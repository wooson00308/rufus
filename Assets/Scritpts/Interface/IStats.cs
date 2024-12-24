using UnityEngine;

public interface IStats
{
    public IntStat Health { get; }
    public IntStat Armor { get;}
    public IntStat AttackDamage { get; }
    public FloatStat AttackSpeed { get; }
    public FloatStat MoveSpeed { get; }
}

public interface IStatSettable
{
    public void UpdateStats(string key, IStats stats);
    public void ResetStats(string key);
}
