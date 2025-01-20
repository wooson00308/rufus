using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent (typeof(Status))]
[RequireComponent (typeof(FSM))]
public class Unit : MonoBehaviour, IStatSettable
{
    public const int MOVE_SPEED_FACTOR = 100;
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
    private readonly Dictionary<int, Skill> _skillDic = new();
    [SerializeField] private Transform _skillStorage;
    private Unit _lastAttacker;
    private Unit _killer;

    private bool _isPlayer;
    private bool _isInitialized;
    private bool _isActive;
    private bool _isRevive;

    public bool IsInitialized => _isInitialized;
    public bool IsActive => _isActive;
    public bool IsRevive => _isRevive;

    public NavMeshAgent Agent => _agent;

    public UnitAnimator Model => _model;

    public Inventory Inventory => _inventory;
    public Status Status => _status;
    public Unit Target => _detector.Target;
    public List<Unit> Targets => _detector.Targets;
    public Unit LastAttacker => _lastAttacker;
    public Unit Killer => _killer;

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

    public void Initialized(UnitData data, Team team = Team.Enemy, bool isPlayer = false)
    {
        _isActive = true;

        Id = data.Id; 
        Name = data.Name;
        Team = team;

        IntializeStats(data);

        _isPlayer = isPlayer;

        if (!_isPlayer)
        {
            _fsm.StartState<IdleState>();
        }
        else
        {
            _fsm.StartState<PlayerIdleState>();
        }
        

        _isInitialized = true;
    }

    public void OnRevive()
    {
        ResetStats(ENGAGE_STATS_KEY);

        if (!_isPlayer)
        {
            _fsm.StartState<IdleState>();
        }
        else
        {
            _fsm.StartState<PlayerIdleState>();
        }
    }

    public void OnDisable()
    {
        _skillDic.Clear();

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

        _lastAttacker = attacker;

        GameEventSystem.Instance.Publish((int)UnitEvents.Hit, new UnitHitEventArgs
        {
            publisher = this,
            attacker = _lastAttacker,
        });

        GameEventSystem.Instance.Publish((int)UnitEvents.Attack, new UnitAttackEventArgs
        {
            publisher = _lastAttacker,
            target = this,
        });

        if (Status.Health.Value <= 0)
        {
            OnDeath(attacker);
        }
    }

    public void OnDeath(Unit attacker)
    {
        if (Status.IsDeath) return;

        if(!_isRevive)
        {
            _status.OnDeath(attacker);

            _lastAttacker = attacker;
            _killer = attacker;
        }

        _fsm.TransitionTo<DeathState>();
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
    public void ApplyStatusFx(StatusFxData data, Unit caster = null)
    {
        var statusFx = ResourceManager.Instance.Spawn(data.Prefab.gameObject, transform)
            .GetComponent<StatusFx>();

        statusFx.OnApply(data, this, caster);
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
        _agent.speed = (Status.MoveSpeed.Value * MOVE_SPEED_FACTOR) * GameTime.DeltaTime;
        _agent.SetDestination(target.position);
        Rotation(target.position - transform.position);
    }

    public void Move(Vector2 vector)
    {
        _agent.isStopped = false;
        _agent.velocity = GameTime.DeltaTime * (Status.MoveSpeed.Value * MOVE_SPEED_FACTOR) * vector;
        Rotation(vector);
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
        _agent.velocity = Vector3.zero;
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

    public void ApplySkill(SkillData skillData)
    {
        if (_skillDic.TryGetValue(skillData.Id, out var skill))
        {
            skill.LevelUp();
        }
        else
        {
            var skillObj = ResourceManager.Instance.Spawn(skillData.Prefab.gameObject);
            var skillComponent = skillObj.GetComponent<Skill>();
            skillComponent.Initialized(this, skillData);

            _skillDic.Add(skillData.Id, skillComponent);

            skillObj.transform.SetParent(_skillStorage);

            GameEventSystem.Instance.Publish((int)SkillEvents.ApplySkill, new SkillEventArgs
            {
                skillId = skillData.Id,
                publisher = this
            });
        }
    }
}