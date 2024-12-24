using UnityEngine;

[RequireComponent (typeof(Animator))]
public class UnitAnimator : MonoBehaviour
{
    private Animator _animator;

    public void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void OnAttack(AnimationEvent e)
    {

    }

    public void OnDeath(AnimationEvent e)
    {

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
    Dash
}