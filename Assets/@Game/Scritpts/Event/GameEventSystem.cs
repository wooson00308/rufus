using System;
using System.Collections.Generic;
using UnityEngine;

public class GameEventSystem : SingletonMini<GameEventSystem>
{
    private readonly Dictionary<string, Action<GameEvent>> _eventDictionary = new();

    public void Subscribe(string eventType, Action<GameEvent> listener)
    {
        if (!_eventDictionary.ContainsKey(eventType))
        {
            _eventDictionary[eventType] = null;
        }

        _eventDictionary[eventType] += listener;
    }

    public void Subscribe(string eventType, params Action<GameEvent>[] listeners)
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

    public void Unsubscribe(string eventType, Action<GameEvent> listener)
    {
        if (_eventDictionary.ContainsKey(eventType))
        {
            _eventDictionary[eventType] -= listener;
            if (_eventDictionary[eventType] == null) // 리스너가 모두 제거되면 삭제
            {
                _eventDictionary.Remove(eventType);
            }
        }
    }

    public void Unsubscribe(string eventType, params Action<GameEvent>[] listeners)
    {
        if (_eventDictionary.ContainsKey(eventType))
        {
            foreach (var listener in listeners)
            {
                _eventDictionary[eventType] -= listener;
            }

            if (_eventDictionary[eventType] == null) // 리스너가 모두 제거되면 삭제
            {
                _eventDictionary.Remove(eventType);
            }
        }
    }

    public void Publish(string eventType, GameEvent gameEvent = null)
    {
        if (_eventDictionary.TryGetValue(eventType, out var action))
        {
            action?.Invoke(gameEvent); // 즉시 실행
        }
    }
}

public class GameEvent
{
    public string eventType;
    public object args;
}