using System.Collections.Generic;
using UnityEngine;

public class ProcessSystem : SingletonMini<ProcessSystem>
{
    public List<Process> _processList = new();

    public Process _prevProcess;

    private void Start()
    {
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