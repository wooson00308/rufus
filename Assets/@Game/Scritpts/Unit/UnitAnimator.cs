using UnityEngine;

[RequireComponent (typeof(Animator))]
public class UnitAnimator : MonoBehaviour
{
    private Unit _owner;
    private Animator _animator;
    private SpriteRenderer _renderer;

    public Animator Animator => _animator;

    public void Awake()
    {
        _owner = GetComponentInParent<Unit>();
        _animator = GetComponent<Animator>();
        _renderer = GetComponent<SpriteRenderer>();
    }

    public void Update()
    {
        _renderer.sortingOrder = (int)(_owner.transform.position.y * -100);
    }

    public void SetMoveSpeed(float speed)
    {
        _animator.SetFloat("MoveSpeed", speed);
    }

    public void SetAttackSpeed(float speed)
    {
        _animator.SetFloat("AttackSpeed", speed);
    }

    public void OnAttack(AnimationEvent e)
    {
        foreach(var target in _owner.Targets)
        {
            target.OnHit(_owner.Status.AttackDamage.Value, _owner);
        }
    }

    public void OnFireProjectile(AnimationEvent e)
    {
        var weapon = _owner.Inventory.GetItemToEquipType(EquipType.Weapon);
        if (weapon == null) return;

        var weaponData = _owner.Inventory.GetItemToEquipType(EquipType.Weapon).Data as WeaponItemData;
        if (weaponData == null) return;

        var projectileData = weaponData.ProjectileData;
        if(projectileData == null) return;

        var projectilePrefab = ResourceManager.Instance.Spawn(projectileData.Prefab.gameObject);
        if(projectilePrefab == null) return;

        var projectile = projectilePrefab.GetComponent<Projectile>();
        projectile.transform.position = transform.position;

        if (projectileData.HasHoming && _owner.Target != null)
        {
            // 타겟이 있고, 유도기능이면 
            projectile.SetTarget(_owner.Target.transform);
        }
        else 
        {
            // 타겟이 없거나 유도기능이 off이면
            var direction = _owner.Model.transform.rotation.y == 1 ? 
                Vector3.left :
                Vector3.right;
            projectile.SetDirection(direction);
        }
        projectile.OnFire(_owner, projectileData);
    }

    public void OnDeath(AnimationEvent e)
    {
        UnitFactory.Instance.DestroyUnit(_owner.GetInstanceID());
    }

    public void CrossFade(string key, float fadeTime = 0)
    {
        _animator.CrossFade(key, fadeTime);
    }
}

public enum UnitState
{
    None,
    Idle,
    Chase,
    Hit,
    Death,
    Attack,
    Dash,
    Casting
}