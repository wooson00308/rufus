using UnityEngine;

[RequireComponent (typeof(Status))]
[RequireComponent (typeof(FSM))]
public class Unit : MonoBehaviour, IStatSettable
{
    public const string BASE_STATS_KEY = "base";
    public const string ENGAGE_STATS_KEY = "engage";

    private Status _status;
    private FSM _fsm;
    private UnitAnimator _animator;

    private bool _isInitialized;
    private bool _isActive;

    public bool IsInitialized => _isInitialized;
    public bool IsActive => _isActive;

    public void Awake()
    {
        _status = GetComponent<Status>();
        _fsm = GetComponent<FSM>();
        _animator = GetComponentInChildren<UnitAnimator>();
    }

    public void Initialized(UnitData data)
    {
        _isActive = true;

        IntializeStats(data);

        UpdateStats(BASE_STATS_KEY, data);

        _fsm.TransitionTo<IdleState>();

        _isInitialized = true;
    }

    public void OnDisable()
    {
        _isInitialized = false;
    }

    public void SetActive(bool value)
    {
        _isActive = value;
    }

    public void IntializeStats(IStats stats)
    {
        _status.IntializeStats(stats);
    }

    public void ResetStats(string key)
    {
        _status.ResetStats(key);
    }

    public void UpdateStats(string key, IStats stats)
    {
        _status.UpdateStats(key, stats);
    }

    public void OnHit(int damage, Unit attacker)
    {
        _status.OnHit(damage, attacker);
    }

    public void OnDeath(Unit attacker)
    {
        _status.OnDeath(attacker);
    }

    public void CrossFade(string key,  float fadeTime = 0)
    {
        _animator.CrossFade(key, fadeTime);
    }
}
