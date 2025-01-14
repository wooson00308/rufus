using UnityEngine;

public class TriggerFxAnimator : MonoBehaviour
{
    public Animator Animator { get; private set; }

    public void Awake()
    {
        Animator = GetComponent<Animator>();
    }

    public void Update()
    {
        Animator.speed = GameTime.TimeScale;
    }

    public void OnDestroyed(AnimationEvent e)
    {
        if(e.stringParameter == "Pool")
        {
            ResourceManager.Instance.Destroy(gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
