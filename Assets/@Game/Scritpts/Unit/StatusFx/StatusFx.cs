using UnityEngine;

public class StatusFx : MonoBehaviour
{
    private Unit _owner; 
    private StatusFxData _data;
    private float _duration;
    private float _elapsedTime = 0;

    private bool _isApplied;

    public void OnApply(StatusFxData data, Unit unit)
    {
        if (!_isApplied) return;

        _data = data;

        _owner= unit;

        _duration = _data.Duration;
        _elapsedTime = 0;

        _data.OnApply(_owner);

        _isApplied = true;
    }

    public void OnDisable()
    {
        _isApplied = false;

        _data.OnRemove(_owner);
    }

    public void Update()
    {
        if (!_isApplied) return;
        if (_data == null) return;

        if(_elapsedTime >= _duration)
        {
            ResourceManager.Instance.Destroy(gameObject);
            return;
        }

        _elapsedTime += Time.deltaTime;

        _data.OnUpdate(_owner);
    }
}
