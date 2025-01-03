using UnityEngine;

[RequireComponent (typeof(Animator))]
public class UnitAnimator : MonoBehaviour
{
    private Unit _owner;
    private Animator _animator;

    public Animator Animator => _animator;

    public void Awake()
    {
        _owner = GetComponentInParent<Unit>();
        _animator = GetComponent<Animator>();
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