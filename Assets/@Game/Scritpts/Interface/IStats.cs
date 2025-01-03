using UnityEngine;

public interface IStats
{
    public IntStat Health { get; }
    public IntStat Armor { get;}
    public IntStat AttackDamage { get; }
    public FloatStat AttackSpeed { get; }
    public FloatStat AttackRange { get; }
    public FloatStat MoveSpeed { get; }

    /// <summary>
    /// 범위공격 범위 / 0이면 단일공격
    /// </summary>
    public FloatStat AoERadius { get; }
}

public interface IStatSettable
{
    public void IntializeStats(IStats stats);
    public void UpdateStats(string key, IStats stats);
    public void ResetStats(string key);
}
