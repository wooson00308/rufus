using UnityEngine;

[RequireComponent (typeof(Status))]
[RequireComponent (typeof(FSM))]
public class Unit : MonoBehaviour, IStatSettable
{
    private const string BASE_STATS_KEY = "base";
    private const string ENGAGE_STATS_KEY = "engage";

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

        UpdateStats(BASE_STATS_KEY, data);

        _fsm.TransitionTo<IdleState>();

        _isInitialized = true;
    }

    public void OnDisable()
    {
        ResetStats(BASE_STATS_KEY);
        ResetStats(ENGAGE_STATS_KEY);
        _isInitialized = false;
    }

    public void SetActive(bool value)
    {
        _isActive = value;
    }

    public void ResetStats(string key)
    {
        _status.ResetStats(key);
    }

    public void UpdateStats(string key, IStats stats)
    {
        _status.UpdateStats(key, stats);
    }

    public void CrossFade(string key,  float fadeTime = 0)
    {
        _animator.CrossFade(key, fadeTime);
    }
}
