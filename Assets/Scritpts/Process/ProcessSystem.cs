using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ProcessSystem : SingletonMini<ProcessSystem>
{
    private List<Process> _processList = new();
    private Process _prevProcess;

    public void Start()
    {
        _processList = GetComponentsInChildren<Process>().ToList();

        foreach (var process in _processList)
        {
            process.Initialized(this);
        }
    }

    public void OnNextProcess<T>() where T : Process
    {
        Process process = _processList.Find(x => x is T);
        if (process == null)
        {
            Debug.LogError($"{typeof(T)} not found");
            return;
        }

        _prevProcess?.SetActive(false);

        process.SetActive(true);

        _prevProcess = process;
    }
}