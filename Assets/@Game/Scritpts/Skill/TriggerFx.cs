using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(CompositeCollider2D))]
[RequireComponent (typeof(Rigidbody2D))]
public class TriggerFx : MonoBehaviour
{
    private Unit _owner;
    private List<Unit> _targets = new();
    private Collider2D[] _colliders;
    private TriggerFxAnimator _model;

    private int _triggerCount = 0;
    private float _duration;
    private float _eps; 
    private bool _onEventFromSelf = false;
    private bool _onEventFromOwnerTriggerFx = false;
    private bool _isInitialized = false;
    private bool _isWaitingForEventDelay;
    private bool _isDestroy = false;

    public Unit Owner => _owner;
    [field: SerializeField] public TriggerFxData Data { get; private set; }
    public Action DestroyEvent { get; set; }
    public Action<Unit, Unit> EnterEvent { get; set; }
    public Action<Unit, Unit> StayEvent { get; set; }
    public Action<Unit, Unit> ExitEvent { get; set; }
    public bool IsDestroy => _isDestroy;

    public void Awake()
    {
        _model = GetComponent<TriggerFxAnimator>();
        _colliders = GetComponents<CompositeCollider2D>();
    }

    public void Initialized(Unit owner)
    {
        _owner = owner;

        _duration = Data.Durtaion;
        _eps = Data.EPS;

        _onEventFromSelf = Data.OnEventFromSelf;
        _onEventFromOwnerTriggerFx = Data.OnEventFromOwnerProjectile;

        StartDuration();

        foreach(var collider in _colliders)
        {
            collider.enabled = true;
        }

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
        _onEventFromOwnerTriggerFx = false;
        _isWaitingForEventDelay = false;
        _isDestroy = false;

        _targets.Clear();
    }

    public async void StartDuration()
    {
        if (_duration == 0) return;

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

        OnDestroyEvent();
    }

    public void OnDestroyEvent()
    {
        if (_isDestroy) return;
        _isDestroy = true;

        foreach (var collider in _colliders)
        {
            collider.enabled = false;
        }

        _model.Animator.CrossFade("Destroy", 0f);

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

            if(Data.MaxMultiTriggerCount > 0 && ++_triggerCount >= Data.MaxMultiTriggerCount)
            {
                OnDestroyEvent();
            }
        }

        else if(collision.TryGetComponent(out TriggerFx triggerFx) && triggerFx.gameObject.activeSelf)
        {
            if (!_onEventFromOwnerTriggerFx && triggerFx.Owner.EqualsUnit(_owner)) return;
            if (Data.MaxMultiTriggerCount > 0 && ++_triggerCount >= Data.MaxMultiTriggerCount)
            {
                OnDestroyEvent();
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
            if (!_onEventFromOwnerTriggerFx && projectile.Owner.EqualsUnit(_owner)) return;
            
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
            if (!_onEventFromOwnerTriggerFx && projectile.Owner.EqualsUnit(_owner)) return;

            ExitEvent?.Invoke(_owner, target);

            _targets.Remove(target);
        }
    }
}
