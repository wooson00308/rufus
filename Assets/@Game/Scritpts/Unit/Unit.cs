using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.Mesh;

[RequireComponent (typeof(Status))]
[RequireComponent (typeof(FSM))]
public class Unit : MonoBehaviour, IStatSettable
{
    public const string BASE_STATS_KEY = "base";
    public const string ENGAGE_STATS_KEY = "engage";

    public int Id { get; private set; }
    public string Name { get; private set; }
    public Team Team { get; private set; }

    private NavMeshAgent _agent;
    private Status _status;
    private FSM _fsm;
    private UnitAnimator _model;
    private Inventory _inventory;
    private TargetDetector _detector;

    private bool _isInitialized;
    private bool _isActive;

    public bool IsInitialized => _isInitialized;
    public bool IsActive => _isActive;

    public NavMeshAgent Agent => _agent;

    public UnitAnimator Model => _model;

    public Inventory Inventory => _inventory;
    public Status Status => _status;
    public Unit Target => _detector.Target;
    public List<Unit> Targets => _detector.Targets;

    public void Awake()
    {
        InitializeComponents();
    }

    private void InitializeComponents()
    {
        _agent = GetComponent<NavMeshAgent>();
        _status = GetComponent<Status>();
        _fsm = GetComponent<FSM>();
        _model = GetComponentInChildren<UnitAnimator>();
        _inventory = GetComponentInChildren<Inventory>();
        _inventory.Initialized(this);
        _detector = GetComponent<TargetDetector>();

        _agent.updateRotation = false;
        _agent.updateUpAxis = false;
    }

    public void Initialized(UnitData data, Team team = Team.Enemy)
    {
        _isActive = true;

        Id = data.Id; 
        Name = data.Name;
        Team = team;

        IntializeStats(data);

        UpdateStats(BASE_STATS_KEY, data);

        _fsm.StartState<IdleState>();

        _isInitialized = true;
    }

    public void OnDisable()
    {
        _isInitialized = false;
        _isActive = false;
    }

    public void SetActive(bool value)
    {
        _isActive = value;
    }

    #region Status
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
        if (Status.IsDeath) return;

        _status.OnHit(damage, attacker);

        if(Status.Health.Value <= 0)
        {
            OnDeath(attacker);
        }
    }

    public void OnDeath(Unit attacker)
    {
        _status.OnDeath(attacker);
    }
    #endregion

    #region Animator
    public void CrossFade(string key,  float fadeTime = 0)
    {
        _model.CrossFade(key, fadeTime);
    }

    public int GetAttackState() => _model.Animator.GetInteger("AttackState");
    public int GetState() => _model.Animator.GetInteger("State");

    #endregion

    #region StatusFx
    public void ApplyStatusFx(StatusFxData data)
    {
        var statusFx = ResourceManager.Instance.Spawn(data.Prefab.gameObject, transform)
            .GetComponent<StatusFx>();

        statusFx.OnApply(data, this);
    }
    #endregion

    #region Movement

    /// <summary>
    /// 타겟 방향으로 이동
    /// </summary>
    /// <param name="target"></param>
    public void MoveFromTarget(Transform target)
    {
        _agent.isStopped = false;
        _agent.speed = Status.MoveSpeed.Value;
        _agent.SetDestination(target.position);
        Rotation(target.position - transform.position);
    }

    public void Warp(Vector3 pos)
    {
        _agent.Warp(pos);
    }

    /// <summary>
    /// 이동 일시중지
    /// </summary>
    public void Stop()
    {
        _agent.isStopped = true;
    }

    /// <summary>
    /// 타겟 초기화
    /// </summary>
    public void ResetMovePath()
    {
        _agent.ResetPath();
    }

    /// <summary>
    /// 해당 방향으로 회전
    /// </summary>
    /// <param name="rotDir"></param>
    public void Rotation(Vector3 rotDir)
    {
        if (rotDir.x != 0)
        {
            float rotY = rotDir.x > 0 ? 0 : 180;
            _model.transform.rotation = Quaternion.Euler(0, rotY, 0);
        }
    }

    #endregion
}
