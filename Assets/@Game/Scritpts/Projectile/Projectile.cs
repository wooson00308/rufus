using UnityEngine;

[RequireComponent(typeof(TriggerFx))]
public class Projectile : MonoBehaviour
{
    private Unit _owner;
    private ProjectileData _data;

    private TriggerFx _triggerFx;
    private bool _isFired;

    private BoxCollider2D _collider;
    private ProjectileAnimator _model;
    private Transform _target;
    private Vector3 _direction;

    private float _speed;

    private bool _isHoming;
    private bool _isDestroyed;

    public Unit Owner => _owner;

    public void Awake()
    {
        _collider = GetComponent<BoxCollider2D>();
        _triggerFx = GetComponent<TriggerFx>();
        _model = GetComponentInChildren<ProjectileAnimator>();
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

        _triggerFx.EnterEvent += _data.OnTriggerEnterEvent;
        _triggerFx.StayEvent += _data.OnTriggerStayEvent;
        _triggerFx.ExitEvent += _data.OnTriggerExitEvent;
        _triggerFx.DestroyEvent += 
            () => {
                _collider.enabled = false;
                _model.Animator.CrossFade("Explosion", 0f);
                _speed = 0;
            };

        _triggerFx.Initialized(owner, _data.TriggerFxData);

        _speed = _data.MoveSpeed.Value;
        _collider.enabled = true;

        _isFired = true;
    }

    public void OnFireOnTarget(Unit owner, ProjectileData data)
    {

    }

    public void OnDestroyed()
    {
        ResourceManager.Instance.Destroy(gameObject);
    }

    public void SetTarget(Transform target)
    {
        _isHoming = true;
        _target = target;
        _direction = (_target.position - transform.position).normalized;
        Rotation(_direction);
    }

    public void SetDirection(Vector3 dir)
    {
        _target = null;
        _direction = dir.normalized;
        Rotation(_direction);
    }

    private void Update()
    {
        if (!_isFired) return;

        if(_isHoming && !_target.gameObject.activeSelf)
        {
            if(!_isDestroyed)
            {
                _isDestroyed = true;
                _collider.enabled = false;
                _model.Animator.CrossFade("Explosion", 0f);
                _speed = 0;
            }

            return;
        }

        if (_target != null)
        {
            _direction = (_target.position - transform.position).normalized;
        }

        transform.position += _speed * Time.deltaTime * _direction;

        if (_direction != Vector3.zero)
        {
            Rotation(_direction);
        }
    }

    public void Rotation(Vector2 rotDir)
    {
        if (!_isFired) return;

        // 방향 벡터의 x축이 양수면 오른쪽, 음수면 왼쪽을 바라보도록 처리
        if (rotDir.x != 0 || rotDir.y != 0)
        {
            float angle = Mathf.Atan2(rotDir.y, rotDir.x) * Mathf.Rad2Deg; // 2D 벡터 방향을 각도로 변환
            _model.transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }
}
