using NavMeshPlus.Components;
using NUnit;
using UnityEngine;

public class Process : MonoBehaviour
{
    protected ProcessSystem _processSystem;

    public GameObject _env;

    public virtual void Initialized(ProcessSystem controller)
    {
        _processSystem = controller;
    }

    public virtual void SetActive(bool value)
    {
        _env.gameObject.SetActive(value);
        
        gameObject.SetActive(value);
    }
}
