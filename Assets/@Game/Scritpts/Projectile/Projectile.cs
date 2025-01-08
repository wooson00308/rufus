using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(TriggerFx))]
public class Projectile : MonoBehaviour
{
    private ProjectileData _data;

    private TriggerFx _triggerFx;
    private NavMeshAgent _agent;
    private bool _isFired;

    private Animator _model;

    public void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _triggerFx = GetComponent<TriggerFx>();
        _model = GetComponentInChildren<Animator>();

        _agent.updateRotation = false;
        _agent.updateUpAxis = false;
    }

    public void OnDisable()
    {
        _isFired = false;
    }

    public void OnFire(Unit owner, ProjectileData data)
    {
        _data = data;

        _triggerFx.EnterEvent += _data.OnTriggerEnterEvent;
        _triggerFx.StayEvent += _data.OnTriggerStayEvent;
        _triggerFx.ExitEvent += _data.OnTriggerExitEvent;
        _triggerFx.Initialized(owner, _data.TriggerFxData);

        _agent.speed = _data.MoveSpeed.Value;

        _isFired = true;
    }

    public void SetTarget(Transform target)
    {
        if (!_isFired) return;

        _agent.isStopped = false;
        _agent.SetDestination(target.position);
        Rotation(target.position - transform.position);
    }

    public void SetDirection(Vector3 dir)
    {
        if (!_isFired) return;

        _agent.isStopped = false;
        _agent.velocity = _data.MoveSpeed.Value * dir;
        Rotation(dir);
    }

    public void Rotation(Vector3 rotDir)
    {
        if (!_isFired) return;

        if (rotDir.x != 0)
        {
            float rotY = rotDir.x > 0 ? 0 : 180;
            _model.transform.rotation = Quaternion.Euler(0, rotY, 0);
        }
    }
}
