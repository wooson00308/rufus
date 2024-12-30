using UnityEngine;

public abstract class StatusFxBase : MonoBehaviour
{
    protected StatusFxData _data;
    protected float _durtaion;
    protected float _elapsedTime = 0;

    protected bool _initalized;

    public void Initialized(StatusFxData data)
    {
        _data = data;

        _durtaion = _data.Duration;
        _elapsedTime = 0;

        OnApply();

        _initalized = true;
    }

    protected virtual void OnDisable()
    {
        OnRemove();

        _initalized = false;
    }

    protected virtual void Update()
    {
        if (!_initalized) return;

        if(_elapsedTime >= _durtaion)
        {
            ResourceManager.Instance.Destroy(gameObject);
            return;
        }

        _elapsedTime += Time.deltaTime;
        OnUpdate();
    }

    protected abstract void OnApply();
    protected abstract void OnUpdate();
    protected abstract void OnRemove();
}
