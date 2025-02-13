using System.Collections.Generic;
using UnityEngine;

public class TargetDetector : MonoBehaviour
{
    private Unit _unit;

    public DetectDataBase _detectData;

    private List<Unit> _enemies;

    public Unit Target
    {
        get
        {
            _enemies = GameFactory.Instance.GetAllActiveUnitsInEnemy(_unit.Team);
            return _detectData.Detect(_unit, _enemies);
        }
    }

    public List<Unit> Targets
    {
        get
        {
            var target = Target;
            if (_unit.Status.AoERadius.Value <= 0) return new List<Unit>() { target };

            var targets = GameFactory.Instance.GetAllActiveUnitsInAoERadius(target, _enemies, _unit.Status.AoERadius.Value);
            return targets;
        }
    }

    private void Awake()
    {
        _unit = GetComponent<Unit>();
    }
}

