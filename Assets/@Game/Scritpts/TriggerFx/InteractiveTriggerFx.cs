using UnityEngine;

public class InteractiveTriggerFx : TriggerFx
{
    public override void Initialize()
    {
        base.Initialize();
        SetupEvents();
    }

    protected override void SetupEvents()
    {
        foreach (var fxData in Data.EnterFxDatas)
        {
            EnterUnitEvent += (owner, target) => fxData.OnEventToTarget(owner, target);
        }

        foreach (var fxData in Data.StayFxDatas)
        {
            StayUnitEvent += (owner, target) => fxData.OnEventToTarget(owner, target);
        }

        foreach (var fxData in Data.ExitFxDatas)
        {
            ExitUnitEvent += (owner, target) => fxData.OnEventToTarget(owner, target);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!_isInitialized) return;

        if (collision.TryGetComponent(out Unit target) && target.IsActive)
        {
            EnterUnitEvent?.Invoke(null, target);
            _targets.Add(target);

            if (Data.OnDestroyEventWhenCollideUnit)
            {
                OnDestroyEvent();
            }
            else if (Data.MaxMultiTriggerCount > 0 && ++_triggerCount >= Data.MaxMultiTriggerCount)
            {
                OnDestroyEvent();
            }

            if(_eps > 0) StartCoroutine(EventPerSeconds(collision));
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!_isInitialized) return;

        if (collision.TryGetComponent(out Unit target) && target.IsActive)
        {
            StayUnitEvent?.Invoke(null, target);

            if (Data.MaxMultiTriggerCount > 0 && ++_triggerCount >= Data.MaxMultiTriggerCount)
            {
                OnDestroyEvent();
            }

            if (_eps > 0) StartCoroutine(EventPerSeconds(collision));
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!_isInitialized) return;

        if (collision.TryGetComponent(out Unit target) && target.IsActive)
        {
            ExitUnitEvent?.Invoke(null, target);
            _targets.Remove(target);
        }
    }
}