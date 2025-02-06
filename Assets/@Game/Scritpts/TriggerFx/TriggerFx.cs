using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(CompositeCollider2D))]
[RequireComponent (typeof(Rigidbody2D))]
public class TriggerFx : MonoBehaviour
{
    private readonly List<Unit> _targets = new();
    private readonly Dictionary<int, bool> _canCollideIds = new();

    private Unit _owner;
    private Collider2D[] _colliders;
    private TriggerFxAnimator _model;

    private int _triggerCount = 0;
    private float _duration;
    private float _eps; 
    private bool _onEventFromSelf = false;
    private bool _onEventFromOwnerTriggerFx = false;
    private bool _isInitialized = false;
    private bool _isDestroy = false;

    public Unit Owner => _owner;
    [field: SerializeField] public TriggerFxData Data { get; private set; }
    public Action DestroyEvent { get; set; }
    public Action<Unit, Unit> EnterUnitEvent { get; set; }
    public Action<Unit, Unit> StayUnitEvent { get; set; }
    public Action<Unit, Unit> ExitUnitEvent { get; set; }
    public bool IsDestroy => _isDestroy;
    public bool _hasNotOwner = false;

    public void Awake()
    {
        _model = GetComponent<TriggerFxAnimator>();
        _colliders = GetComponents<CompositeCollider2D>();

        if(_hasNotOwner)
        {
            foreach (var fxData in Data.EnterFxDatas)
            {
                EnterUnitEvent += fxData.OnEventToTarget;
            }

            foreach (var fxData in Data.StayFxDatas)
            {
                StayUnitEvent += fxData.OnEventToTarget;
            }

            foreach (var fxData in Data.ExitFxDatas)
            {
                ExitUnitEvent += fxData.OnEventToTarget;
            }

            _duration = Data.Durtaion;
            _eps = Data.EPS;

            _onEventFromSelf = Data.OnEventFromSelf;
            _onEventFromOwnerTriggerFx = Data.OnEventFromOwnerProjectile;
        }
    }

    public void Initialized(Unit owner)
    {
        _owner = owner;

        foreach (var fxData in Data.EnterFxDatas)
        {
            EnterUnitEvent += fxData.OnEventToTarget;
        }

        foreach (var fxData in Data.StayFxDatas)
        {
            StayUnitEvent += fxData.OnEventToTarget;
        }

        foreach (var fxData in Data.ExitFxDatas)
        {
            ExitUnitEvent += fxData.OnEventToTarget;
        }

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
        if(!_hasNotOwner)
        {
            EnterUnitEvent = null;
            StayUnitEvent = null;
            ExitUnitEvent = null;

            _triggerCount = 0;
            _isInitialized = false;
            _onEventFromSelf = false;
            _onEventFromOwnerTriggerFx = false;
            _isDestroy = false;
        }

        _targets.Clear();
        _canCollideIds.Clear();
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

            time += GameTime.DeltaTime;

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

    private bool CanCollide(Collider2D coll)
    {
        if (!_canCollideIds.TryGetValue(coll.GetInstanceID(), out bool canCollide))
        {
            _canCollideIds.Add(coll.GetInstanceID(), true);
            return true;
        }

        return canCollide;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (!CanCollide(collision)) return;

        if(_hasNotOwner)
        {
            if (collision.TryGetComponent(out Unit target) && target.IsActive)
            {
                EnterUnitEvent?.Invoke(_owner, target);

                _targets.Add(target);

                if (Data.OnDestroyEventWhenCollideUnit)
                {
                    OnDestroyEvent();
                }

                else if (Data.MaxMultiTriggerCount > 0 && ++_triggerCount >= Data.MaxMultiTriggerCount)
                {
                    OnDestroyEvent();
                }

                StartCoroutine(EventPerSeconds(collision));
            }

            else if (collision.TryGetComponent(out TriggerFx triggerFx) && triggerFx.gameObject.activeSelf)
            {
                if (Data.MaxMultiTriggerCount > 0 && ++_triggerCount >= Data.MaxMultiTriggerCount)
                {
                    OnDestroyEvent();
                }

                StartCoroutine(EventPerSeconds(collision));
            }
        }
        else
        {
            if (!_isInitialized) return;
            if (collision.TryGetComponent(out Unit target) && target.IsActive)
            {
                if (!_onEventFromSelf && _owner.EqualsUnit(target)) return;
                if (_owner.Team == target.Team) return;

                EnterUnitEvent?.Invoke(_owner, target);

                _targets.Add(target);

                if (Data.OnDestroyEventWhenCollideUnit)
                {
                    OnDestroyEvent();
                }

                else if (Data.MaxMultiTriggerCount > 0 && ++_triggerCount >= Data.MaxMultiTriggerCount)
                {
                    OnDestroyEvent();
                }

                StartCoroutine(EventPerSeconds(collision));
            }

            else if (collision.TryGetComponent(out TriggerFx triggerFx) && triggerFx.gameObject.activeSelf)
            {
                if (!_onEventFromOwnerTriggerFx && triggerFx.Owner.EqualsUnit(_owner)) return;
                if (_owner.Team == triggerFx.Owner.Team) return;

                if (Data.MaxMultiTriggerCount > 0 && ++_triggerCount >= Data.MaxMultiTriggerCount)
                {
                    OnDestroyEvent();
                }

                StartCoroutine(EventPerSeconds(collision));
            }
        }
    }

    public void OnTriggerStay2D(Collider2D collision)
    {
        if (!CanCollide(collision)) return;

        if(_hasNotOwner)
        {
            if (collision.TryGetComponent(out Unit target) && target.IsActive)
            {
                StayUnitEvent?.Invoke(_owner, target);

                if (Data.MaxMultiTriggerCount > 0 && ++_triggerCount >= Data.MaxMultiTriggerCount)
                {
                    OnDestroyEvent();
                }

                StartCoroutine(EventPerSeconds(collision));
            }

            else if (collision.TryGetComponent(out TriggerFx triggerFx) && triggerFx.gameObject.activeSelf)
            {
                StayUnitEvent?.Invoke(_owner, target);

                if (Data.MaxMultiTriggerCount > 0 && ++_triggerCount >= Data.MaxMultiTriggerCount)
                {
                    OnDestroyEvent();
                }

                StartCoroutine(EventPerSeconds(collision));
            }
        }
        else
        {
            if (!_isInitialized) return;
            if (collision.TryGetComponent(out Unit target) && target.IsActive)
            {
                if (!_onEventFromSelf && _owner.EqualsUnit(target)) return;
                if (_owner.Team == target.Team) return;

                StayUnitEvent?.Invoke(_owner, target);

                if (Data.MaxMultiTriggerCount > 0 && ++_triggerCount >= Data.MaxMultiTriggerCount)
                {
                    OnDestroyEvent();
                }

                StartCoroutine(EventPerSeconds(collision));
            }

            else if (collision.TryGetComponent(out TriggerFx triggerFx) && triggerFx.gameObject.activeSelf)
            {
                if (!_onEventFromOwnerTriggerFx && triggerFx.Owner.EqualsUnit(_owner)) return;
                if (_owner.Team == triggerFx.Owner.Team) return;

                StayUnitEvent?.Invoke(_owner, target);

                if (Data.MaxMultiTriggerCount > 0 && ++_triggerCount >= Data.MaxMultiTriggerCount)
                {
                    OnDestroyEvent();
                }

                StartCoroutine(EventPerSeconds(collision));
            }
        }
        
    }

    private IEnumerator EventPerSeconds(Collider2D coll)
    {
        _canCollideIds[coll.GetInstanceID()] = false;

        float time = 0;
        while(time < _eps)
        {
            time += GameTime.DeltaTime;
            yield return null;

            if (!_isInitialized) yield break;
        }

        _canCollideIds[coll.GetInstanceID()] = true;
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (!CanCollide(collision)) return;

        if(_hasNotOwner)
        {
            if (collision.TryGetComponent(out Unit target) && target.IsActive)
            {
                ExitUnitEvent?.Invoke(_owner, target);

                _targets.Remove(target);
            }

            else if (collision.TryGetComponent(out TriggerFx triggerFx) && triggerFx.gameObject.activeSelf)
            {
                //TODO
            }
        }
        else
        {
            if (!_isInitialized) return;
            if (collision.TryGetComponent(out Unit target) && target.IsActive)
            {
                if (!_onEventFromSelf && _owner.EqualsUnit(target)) return;
                if (_owner.Team == target.Team) return;

                ExitUnitEvent?.Invoke(_owner, target);

                _targets.Remove(target);
            }

            else if (collision.TryGetComponent(out TriggerFx triggerFx) && triggerFx.gameObject.activeSelf)
            {
                if (!_onEventFromOwnerTriggerFx && triggerFx.Owner.EqualsUnit(_owner)) return;
                if (_owner.Team == triggerFx.Owner.Team) return;

                //TODO
            }
        }
        
    }
}
