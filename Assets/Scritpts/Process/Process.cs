using UnityEngine;

public class Process : MonoBehaviour
{
    protected ProcessSystem _processSystem;

    public virtual void Initialized(ProcessSystem controller)
    {
        _processSystem = controller;
    }

    public virtual void SetActive(bool value)
    {
        gameObject.SetActive(value);
    }
}
