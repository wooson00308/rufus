using System;
using System.Collections.Generic;
using UnityEngine;

public class TriggerFx : MonoBehaviour
{
    private TriggerFxData _data;
    private Unit _owner;
    private List<Unit> _targets = new();

    private int _triggerCount = 0;
    private float _duration;
    private float _eps; // Event per seconds
    private bool _onEventFromSelf = false;
    private bool _onEventFromOwnerProjectile = false;
    private bool _isInitialized = false;
    private bool _isWaitingForEventDelay;

    public Action DestroyEvent { get; set; }
    public Action<Unit, Unit> EnterEvent { get; set; }
    public Action<Unit, Unit> StayEvent { get; set; }
    public Action<Unit, Unit> ExitEvent { get; set; }

    public void Initialized(Unit owner, TriggerFxData data)
    {
        _owner = owner;
        _data = data;

        _duration = data.Durtaion;
        _eps = data.EPS;

        _onEventFromSelf = data.OnEventFromSelf;
        _onEventFromOwnerProjectile = data.OnEventFromOwnerProjectile;

        StartDuration();

        _isInitialized = true;
    }

    public void OnDisable()
    {
        EnterEvent = null;
        StayEvent = null;
        ExitEvent = null;

        _triggerCount = 0;
        _isInitialized = false;
        _onEventFromSelf = false;
        _onEventFromOwnerProjectile = false;
        _isWaitingForEventDelay = false;

        _targets.Clear();
    }

    public async void StartDuration()
    {
        float time = 0;
        while(time <= _duration)
        {
            if(!gameObject.activeSelf)
            {
                return;
            }

            time += Time.deltaTime;

            await Awaitable.EndOfFrameAsync();
        }

        DestroyEvent?.Invoke();
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (!_isInitialized) return;
        if(collision.TryGetComponent(out Unit target) && target.IsActive)
        {
            if (!_onEventFromSelf && _owner.EqualsUnit(target)) return;

            EnterEvent?.Invoke(_owner, target);

            _targets.Add(target);

            if(++_triggerCount >= _data.MaxMultiTriggerCount)
            {
                DestroyEvent?.Invoke();
            }
        }

        else if(collision.TryGetComponent(out Projectile projectile) && projectile.gameObject.activeSelf)
        {
            if (!_onEventFromOwnerProjectile && projectile.Owner.EqualsUnit(_owner)) return;
            if (++_triggerCount >= _data.MaxMultiTriggerCount)
            {
                DestroyEvent?.Invoke();
            }
        }
    }

    public void OnTriggerStay2D(Collider2D collision)
    {
        if (_isWaitingForEventDelay) return;
        if (!_isInitialized) return;
        if (collision.TryGetComponent(out Unit target) && target.IsActive)
        {
            if (!_onEventFromSelf && _owner.EqualsUnit(target)) return;

            StayEvent?.Invoke(_owner, target);

            EventPerSeconds();
        }

        else if (collision.TryGetComponent(out Projectile projectile) && projectile.gameObject.activeSelf)
        {
            if (!_onEventFromOwnerProjectile && projectile.Owner.EqualsUnit(_owner)) return;
            
            StayEvent?.Invoke(_owner, target);

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
            if (!_onEventFromSelf && _owner.EqualsUnit(target)) return;

            ExitEvent?.Invoke(_owner, target);

            _targets.Remove(target);
        }

        else if (collision.TryGetComponent(out Projectile projectile) && projectile.gameObject.activeSelf)
        {
            if (!_onEventFromOwnerProjectile && projectile.Owner.EqualsUnit(_owner)) return;

            ExitEvent?.Invoke(_owner, target);

            _targets.Remove(target);
        }
    }
}
