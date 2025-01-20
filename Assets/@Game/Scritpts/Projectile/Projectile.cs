using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private Transform _model;
    private Unit _owner;
    private ProjectileData _data;
    private Rigidbody2D _rigidbody;

    private TriggerFx[] _triggerFxs;
    private bool _isFired;

    private Transform _target;
    private Vector3 _direction;

    private float _speed;
    private float _duration;
    private float _rotationSpeed;

    private bool _isHoming;
    private bool _isDestroyed;

    public Unit Owner => _owner;

    public void Awake()
    {
        _triggerFxs = GetComponentsInChildren<TriggerFx>();
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    public void OnDisable()
    {
        _isDestroyed = false;
        _isHoming = false;
        _isFired = false;
        _target = null;
        _direction = Vector3.zero;
    }

    public void OnFire(Unit owner, ProjectileData data)
    {
        _owner = owner;
        _data = data;

        _duration = _data.Duration.Value;
        _rotationSpeed = _data.RotationSpeed.Value;

        foreach (var triggerFx in _triggerFxs)
        {
            triggerFx.gameObject.SetActive(true);
            triggerFx.EnterUnitEvent += _data.OnTriggerEnterEvent;
            triggerFx.StayUnitEvent += _data.OnTriggerStayEvent;
            triggerFx.ExitUnitEvent += _data.OnTriggerExitEvent;
            triggerFx.DestroyEvent +=
                () => {
                    bool isAllDestroyed = true;

                    foreach (var triggerFx in _triggerFxs)
                    {
                        if (!triggerFx.IsDestroy)
                        {
                            isAllDestroyed = false;
                            break;
                        }
                    }

                    if (isAllDestroyed)
                    {
                        _speed = 0;
                    }
                };

            triggerFx.Initialized(owner);
        }

        _speed = _data.MoveSpeed.Value;

        StartDuration();

        _isFired = true;
    }

    public void OnFireOnTarget(Unit owner, ProjectileData data)
    {

    }

    public async void StartDuration()
    {
        if (_duration == 0) return;

        float time = 0;
        while (time <= _duration)
        {
            if (!gameObject.activeSelf)
            {
                return;
            }

            time += GameTime.DeltaTime;

            await Awaitable.EndOfFrameAsync();
        }

        OnDestroyFx();
    }

    public void SetTarget(Transform target)
    {
        _isHoming = true;
        _target = target;
    }

    public void SetDirection(Vector3 dir)
    {
        _isHoming = false;
        _target = null;
        _direction = dir.normalized;
    }

    public void Update()
    {
        if (!_isFired) return;

        CheckFxStates();
        Move();
    }

    private bool CheckFxStates()
    {
        bool isAllDeactive = true;

        foreach (var triggerFx in _triggerFxs)
        {
            if (triggerFx.gameObject.activeSelf)
            {
                isAllDeactive = false;
                break;
            }
        }

        if (isAllDeactive)
        {
            OnDestroyed();
            return false;
        }

        return true;
    }

    private void Move()
    {
        if (_isHoming && (!_target || !_target.gameObject.activeSelf))
        {
            SetDirection(_direction);

            return;
        }

        Vector2 targetDirection = _direction;
        if (_target != null)
        {
            targetDirection = (_target.position - transform.position).normalized;
        }

        if (_isHoming)
        {
            _direction = Vector2.Lerp(_direction, targetDirection, _rotationSpeed * Time.fixedDeltaTime).normalized;
        }
        else
        {
            _direction = targetDirection;
        }

        _rigidbody.linearVelocity = _direction * _speed;

        Rotation(_direction);
    }

    public void Rotation(Vector2 rotDir)
    {
        if (_model == null) return;
        if (!_isFired) return;

        if (rotDir.x != 0 || rotDir.y != 0)
        {
            float angle = Mathf.Atan2(rotDir.y, rotDir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    public void OnDestroyFx()
    {
        if (!_isDestroyed)
        {
            _isDestroyed = true;
            _speed = 0;

            foreach (var triggerFx in _triggerFxs)
            {
                if (!triggerFx.gameObject.activeSelf) continue;
                triggerFx.OnDestroyEvent();
            }
        }
    }

    public void OnDestroyed()
    {
        ResourceManager.Instance.Destroy(gameObject);
    }
}
