using UnityEngine;

public class ProjectileTriggerFx : TriggerFx
{
    private Unit _owner;
    private bool _onEventFromSelf;
    private bool _onEventFromOwnerTriggerFx;

    public Unit Owner => _owner;

    public void Initialized(Unit owner)
    {
        _owner = owner;
        _onEventFromSelf = Data.OnEventFromSelf;
        _onEventFromOwnerTriggerFx = Data.OnEventFromOwnerProjectile;
        base.Initialize();
        SetupEvents();
    }

    protected override void SetupEvents()
    {
        foreach (var fxData in Data.EnterFxDatas)
        {
            EnterUnitEvent += (owner, target) => fxData.OnEventToTarget(_owner, target);
        }

        foreach (var fxData in Data.StayFxDatas)
        {
            StayUnitEvent += (owner, target) => fxData.OnEventToTarget(_owner, target);
        }

        foreach (var fxData in Data.ExitFxDatas)
        {
            ExitUnitEvent += (owner, target) => fxData.OnEventToTarget(_owner, target);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!CanCollide(collision) || !_isInitialized) return;

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
            if (!_onEventFromOwnerTriggerFx && triggerFx is ProjectileTriggerFx projFx && projFx.Owner.EqualsUnit(_owner)) return;
            if (triggerFx is ProjectileTriggerFx pFx && _owner.Team == pFx.Owner.Team) return;

            if (Data.MaxMultiTriggerCount > 0 && ++_triggerCount >= Data.MaxMultiTriggerCount)
            {
                OnDestroyEvent();
            }

            StartCoroutine(EventPerSeconds(collision));
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!CanCollide(collision) || !_isInitialized) return;

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
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!CanCollide(collision) || !_isInitialized) return;

        if (collision.TryGetComponent(out Unit target) && target.IsActive)
        {
            if (!_onEventFromSelf && _owner.EqualsUnit(target)) return;
            if (_owner.Team == target.Team) return;

            ExitUnitEvent?.Invoke(_owner, target);
            _targets.Remove(target);
        }
    }
}