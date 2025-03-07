using System.Collections.Generic;
using System;
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public abstract class TriggerFx : MonoBehaviour
{
    protected readonly List<Unit> _targets = new();
    protected readonly Dictionary<int, bool> _canCollideIds = new();

    protected Collider2D[] _colliders;
    protected TriggerFxAnimator _model;

    protected int _triggerCount = 0;
    protected float _duration;
    protected float _eps;
    protected bool _isInitialized = false;
    protected bool _isDestroy = false;
    

    [field: SerializeField] public TriggerFxData Data { get; private set; }
    public Action DestroyEvent { get; set; }
    public Action<Unit, Unit> EnterUnitEvent { get; set; } // Unit만 전달하도록 변경
    public Action<Unit, Unit> StayUnitEvent { get; set; }
    public Action<Unit, Unit> ExitUnitEvent { get; set; }
    public bool IsDestroy => _isDestroy;
    public bool _isSpanwed = true;

    protected virtual void Awake()
    {
        _model = GetComponent<TriggerFxAnimator>();
        _colliders = GetComponents<Collider2D>();
    }

    private void OnEnable()
    {
        if (!_isSpanwed)
        {
            Initialize();
        }
    }

    public virtual void Initialize()
    {
        _isInitialized = true;

        _duration = Data.Duration;
        _eps = Data.EPS;

        StartDuration();

        foreach (var collider in _colliders)
        {
            collider.enabled = true;
        }
    }

    protected virtual void OnDisable()
    {
        EnterUnitEvent = null;
        StayUnitEvent = null;
        ExitUnitEvent = null;

        _triggerCount = 0;
        _isInitialized = false;
        _isDestroy = false;

        _targets.Clear();
        _canCollideIds.Clear();
    }

    protected async void StartDuration()
    {
        if (_duration == 0) return;

        float time = 0;
        while (time <= _duration)
        {
            if (!gameObject.activeSelf)
            {
                return;
            }

            time += GameTime.DeltaTime;
            await Awaitable.EndOfFrameAsync();
        }

        OnDestroyEvent();
    }

    public virtual void OnDestroyEvent()
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

    protected bool CanCollide(Collider2D coll)
    {
        if (!_canCollideIds.TryGetValue(coll.GetInstanceID(), out bool canCollide))
        {
            _canCollideIds.Add(coll.GetInstanceID(), true);
            return true;
        }
        return canCollide;
    }

    protected IEnumerator EventPerSeconds(Collider2D coll)
    {
        _canCollideIds[coll.GetInstanceID()] = false;

        float time = 0;
        while (time < _eps)
        {
            time += GameTime.DeltaTime;
            yield return null;

            if (!_isInitialized) yield break;
        }

        _canCollideIds[coll.GetInstanceID()] = true;
    }

    protected abstract void SetupEvents();
}