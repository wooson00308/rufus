using System;
using System.Collections.Generic;

/// <summary>
/// TEnum 타입이 반드시 enum 이어야만 하도록 제한
/// </summary>
public class GameEventSystem : Singleton<GameEventSystem>
{
    private readonly Dictionary<int, Action<object>> _eventDictionary = new();

    /// <summary>
    /// 단일 리스너 등록
    /// </summary>
    public void Subscribe(int eventType, Action<object> listener)
    {
        if (!_eventDictionary.ContainsKey(eventType))
        {
            _eventDictionary[eventType] = null;
        }
        _eventDictionary[eventType] += listener;
    }

    /// <summary>
    /// 여러 리스너 등록
    /// </summary>
    public void Subscribe(int eventType, params Action<object>[] listeners)
    {
        if (!_eventDictionary.ContainsKey(eventType))
        {
            _eventDictionary[eventType] = null;
        }
        foreach (var listener in listeners)
        {
            _eventDictionary[eventType] += listener;
        }
    }

    /// <summary>
    /// 단일 리스너 해제
    /// </summary>
    public void Unsubscribe(int eventType, Action<object> listener)
    {
        if (_eventDictionary.ContainsKey(eventType))
        {
            _eventDictionary[eventType] -= listener;
            if (_eventDictionary[eventType] == null)
            {
                _eventDictionary.Remove(eventType);
            }
        }
    }

    /// <summary>
    /// 여러 리스너 해제
    /// </summary>
    public void Unsubscribe(int eventType, params Action<object>[] listeners)
    {
        if (_eventDictionary.ContainsKey(eventType))
        {
            foreach (var listener in listeners)
            {
                _eventDictionary[eventType] -= listener;
            }
            if (_eventDictionary[eventType] == null)
            {
                _eventDictionary.Remove(eventType);
            }
        }
    }

    /// <summary>
    /// 이벤트 발행
    /// </summary>
    public void Publish(int eventType, object gameEvent = null)
    {
        if (_eventDictionary.TryGetValue(eventType, out var action))
        {
            action?.Invoke(gameEvent);
        }
    }
}
