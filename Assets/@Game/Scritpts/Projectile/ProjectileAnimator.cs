using UnityEngine;

public class ProjectileAnimator : MonoBehaviour
{
    private Projectile _projectile;
    public Animator Animator { get; private set; }

    public void Awake()
    {
        Animator = GetComponent<Animator>();
        _projectile = GetComponentInParent<Projectile>();
    }

    public void OnDestroyed(AnimationEvent e)
    {
        _projectile.OnDestroyed();
    }
}
