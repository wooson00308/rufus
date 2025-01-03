using System.Collections.Generic;
using System;
using UnityEngine;

public enum StatValueType
{
    None,
    Max,
    Min,
}

public abstract class Stat<T>
{
    [SerializeField] protected T _value;

    private bool _isUsedMinValue = false;
    private bool _isUsedMaxValue = false;

    private T _minValue;
    private T _maxValue;

    protected Dictionary<(string, StatValueType), T> _updateStats = new();

    protected T Clamp(T value)
    {
        bool isLessThanMin = Comparer<T>.Default.Compare(value, _minValue) <= 0 && _isUsedMinValue;
        bool isGreaterThanMax = Comparer<T>.Default.Compare(value, _maxValue) > 0 && _isUsedMaxValue;

        if (isLessThanMin)
        {
            return _minValue;
        }
        else if (isGreaterThanMax)
        {
            return _maxValue;
        }
        else
        {
            return value;
        }
    }

    public event Action<T> OnValueChanged;

    public T Value => _value;

    public T Max => _maxValue;
    public T Min => _minValue;

    public Stat(T value)
    {
        _value = value;
        _updateStats.Clear();
    }

    public void SetMaxValue(T value)
    {
        _maxValue = value;
        _isUsedMaxValue = true;
    }

    public void SetMinValue(T value)
    {
        _minValue = value;
        _isUsedMinValue = true;
    }

    protected void SetValue(T value)
    {
        _value = Clamp(value);
        OnValueChanged?.Invoke(_value);
    }

    public virtual void Reset(string key, StatValueType type = StatValueType.None)
    {
        if (!_updateStats.ContainsKey((key, type)))
        {
            return;
        }

        T subtract = Subtract(_value, _updateStats[(key, type)]);
        SetValue(subtract);

        _updateStats.Remove((key, type));
    }

    public virtual void Update(string key, T value, StatValueType type = StatValueType.None)
    {
        if (_updateStats.ContainsKey((key, type)))
        {
            _updateStats[(key, type)] = Add(_updateStats[(key, type)], value);
        }
        else
        {
            _updateStats.Add((key, type), value);
        }

        T add = Add(_value, value);
        SetValue(add);
    }

    protected abstract T Add(T a, T b);
    protected abstract T Subtract(T a, T b);
}

[Serializable]
public class IntStat : Stat<int>
{
    public IntStat(int value) : base(value)
    {
    }

    protected override int Add(int a, int b) => a + b;
    protected override int Subtract(int a, int b) => a - b;
}

[Serializable]
public class FloatStat : Stat<float>
{
    public FloatStat(float value) : base(value)
    {
    }

    protected override float Add(float a, float b) => a + b;
    protected override float Subtract(float a, float b) => a - b;
}