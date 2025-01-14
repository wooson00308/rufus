using Unity.VisualScripting;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private Transform _model;
    private Unit _owner;
    private ProjectileData _data;

    private TriggerFx[] _triggerFxs;
    private bool _isFired;

    private Transform _target;
    private Vector3 _direction;

    private float _speed;
    private float _duration;

    private bool _isHoming;
    private bool _isDestroyed;

    public Unit Owner => _owner;

    public void Awake()
    {
        _triggerFxs = GetComponentsInChildren<TriggerFx>();
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

        OnDestroyed();
    }

    public void SetTarget(Transform target)
    {
        _isHoming = true;
        _target = target;
        _direction = (_target.position - transform.position).normalized;
    }

    public void SetDirection(Vector3 dir)
    {
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
        if (_isHoming && !_target.gameObject.activeSelf)
        {
            if (!_isDestroyed)
            {
                _isDestroyed = true;

                foreach (var triggerFx in _triggerFxs)
                {
                    if (!triggerFx.gameObject.activeSelf) continue;
                    triggerFx.OnDestroyEvent();
                }
            }

            return;
        }

        if (_target != null)
        {
            _direction = (_target.position - transform.position).normalized;
        }

        transform.position += _speed * GameTime.DeltaTime * _direction;
        Rotation(_direction);
    }

    public void Rotation(Vector2 rotDir)
    {
        if (_model == null) return;
        if (!_isFired) return;

        // 방향 벡터의 x축이 양수면 오른쪽, 음수면 왼쪽을 바라보도록 처리
        if (rotDir.x != 0 || rotDir.y != 0)
        {
            float angle = Mathf.Atan2(rotDir.y, rotDir.x) * Mathf.Rad2Deg; // 2D 벡터 방향을 각도로 변환
            _model.transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    public void OnDestroyed()
    {
        ResourceManager.Instance.Destroy(gameObject);
    }
}
