using System;
using System.Collections.Generic;
using UnityEngine;

public class TriggerFx : MonoBehaviour
{
    private Unit _owner;
    private List<Unit> _targets = new();

    private float _duration;
    private float _eps; // Event per seconds
    private bool _isOwnerTrigger = false;
    private bool _isInitialized = false;
    private bool _isWaitingForEventDelay;

    public Action<Unit> EnterEvent { get; set; }
    public Action<Unit> StayEvent { get; set; }
    public Action<Unit> ExitEvent { get; set; }

    public void Initialized(Unit owner, float duration, float eps, bool isOwnerTrigger = false)
    {
        _owner = owner;

        _duration = duration;
        _eps = eps;

        _isOwnerTrigger = isOwnerTrigger;
        _isInitialized = true;
    }

    public void OnDisable()
    {
        EnterEvent = null;
        StayEvent = null;
        ExitEvent = null;

        _isInitialized = false;
        _isOwnerTrigger = false;
        _isWaitingForEventDelay = false;
    }

    public async void Duration()
    {
        await Awaitable.WaitForSecondsAsync(_duration);

        ResourceManager.Instance.Destroy(gameObject);
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (!_isInitialized) return;
        if(collision.TryGetComponent(out Unit target) && target.IsActive)
        {
            if (_isOwnerTrigger && _owner.EqualsUnit(target)) return;

            EnterEvent?.Invoke(target);

            _targets.Add(target);
        }
    }

    public void OnTriggerStay2D(Collider2D collision)
    {
        if (_isWaitingForEventDelay) return;
        if (!_isInitialized) return;
        if (collision.TryGetComponent(out Unit target) && target.IsActive)
        {
            if (_isOwnerTrigger && _owner.EqualsUnit(target)) return;

            StayEvent?.Invoke(target);

            EventPerSeconds();
        }
    }

    private async void EventPerSeconds()
    {
        _isWaitingForEventDelay = true;
        await Awaitable.WaitForSecondsAsync(_eps);
        _isWaitingForEventDelay = false;
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (!_isInitialized) return;
        if (collision.TryGetComponent(out Unit target) && target.IsActive)
        {
            if (_isOwnerTrigger && _owner.EqualsUnit(target)) return;

            ExitEvent?.Invoke(target);

            _targets.Remove(target);
        }
    }
}
