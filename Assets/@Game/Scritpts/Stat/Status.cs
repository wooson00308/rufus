using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Status : MonoBehaviour, IStats, IStatSettable
{
    private Unit _owner;

    [HideInInspector] public IntStat Health { get; private set; }
    [HideInInspector] public IntStat Mana { get; private set; }
    [HideInInspector] public IntStat ManaRegenPerSeconds { get; private set; }
    [HideInInspector] public IntStat Armor { get; private set; }
    [HideInInspector] public IntStat AttackDamage { get; private set; }
    [HideInInspector] public FloatStat AttackSpeed { get; private set; }
    [HideInInspector] public FloatStat AttackRange { get; private set; }
    [HideInInspector] public FloatStat MoveSpeed { get; private set; }
    [HideInInspector] public FloatStat AoERadius { get; private set; }

    public bool IsStun { get; private set; }
    public bool IsDeath { get; private set; }

    public void Awake()
    {
        _owner = GetComponent<Unit>();
    }

    public void Start()
    {
        StartCoroutine(LoopManaRegeneration());
    }

    public void IntializeStats(IStats stats)
    {
        Health = new IntStat(stats.Health.Value);
        Mana = new IntStat(stats.Mana.Value);
        Mana.SetMaxValue(stats.Mana.Value);
        ManaRegenPerSeconds = new IntStat(stats.ManaRegenPerSeconds.Value);
        Armor = new IntStat(stats.Armor.Value);
        AttackDamage = new IntStat(stats.AttackDamage.Value);
        AttackRange = new FloatStat(stats.AttackRange.Value);
        AttackSpeed = new FloatStat(stats.AttackSpeed.Value);
        AttackSpeed.OnValueChanged += 
            (value) => {
                _owner.Model.SetAttackSpeed(value); 
            };

        MoveSpeed = new FloatStat(stats.MoveSpeed.Value);
        MoveSpeed.OnValueChanged +=
            (value) => {
                _owner.Agent.speed = (float)value;
                _owner.Model.SetMoveSpeed(value);
            };

        AoERadius = new FloatStat(stats.AoERadius.Value);
    }

    public void ResetStats(string key)
    {
        Health.Reset(key);
        //Mana.Reset(key);
        Mana.Reset(key, StatValueType.Max);
        ManaRegenPerSeconds.Reset(key);
        Armor.Reset(key);
        AttackDamage.Reset(key);
        AttackSpeed.Reset(key);
        AttackRange.Reset(key);
        MoveSpeed.Reset(key);
        AoERadius.Reset(key);
    }

    public void UpdateStats(string key, IStats stats)
    {
        Health.Update(key, stats.Health.Value);
        Mana.Update(key, stats.Mana.Value, StatValueType.Max);
        ManaRegenPerSeconds.Update(key, stats.ManaRegenPerSeconds.Value);
        Armor.Update(key, stats.Armor.Value);
        AttackDamage.Update(key, stats.AttackDamage.Value);
        AttackSpeed.Update(key, stats.AttackSpeed.Value);
        AttackRange.Update(key, stats.AttackRange.Value);
        MoveSpeed.Update(key, stats.MoveSpeed.Value);
        AoERadius.Update(key, stats.AoERadius.Value);
    }

    public void OnStun(Unit attacker)
    {
        if (IsStun) return;
        if (IsDeath) return;

        IsStun = true;
    }

    public void OnHit(int damage, Unit attacker)
    {
        if (IsDeath) return;

        Health.Update(Unit.ENGAGE_STATS_KEY, -damage);
    }

    public void OnDeath(Unit attacker)
    {
        if (IsDeath) return;
        IsDeath = true;
    }

    private IEnumerator LoopManaRegeneration()
    {
        var waitForSeconds = new WaitForSeconds(1);
        while (true)
        {
            yield return waitForSeconds;

            if (_owner == null) continue;
            if (!_owner.IsActive) continue;
            if (IsDeath) continue;
            if (Mana.Value >= Mana.Max) continue;
            
            Mana.Update(Unit.ENGAGE_STATS_KEY, ManaRegenPerSeconds.Value);
        }
    }
}
