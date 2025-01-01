using System.Collections.Generic;
using UnityEngine;

public class TargetDetector : MonoBehaviour
{
    private Unit _unit;

    public DetectDataBase _detectData;

    private Unit _target;

    public List<Unit> Targets
    {
        get
        {
            var units = UnitFactory.Instance.GetAllActiveUnitsInEnemy(_unit.Team);
            _target = _detectData.Detect(_unit, units, _target);

            if(_unit.Status.AoERadius.Value <= 0) return new List<Unit>() { _target };

            var targets = UnitFactory.Instance.GetAllActiveUnitsInAoERadius(_target, units, _unit.Status.AoERadius.Value);

            return targets;
        }
    }

    private void Awake()
    {
        _unit = GetComponent<Unit>();
    }
}

