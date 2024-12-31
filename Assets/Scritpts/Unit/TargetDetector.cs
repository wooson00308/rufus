using System.Collections.Generic;
using UnityEngine;

public class TargetDetector : MonoBehaviour
{
    private Unit _unit;

    public DetectDataBase _detectData;

    private Unit _currentTarget;

    public List<Unit> Target
    {
        get
        {
            var units = UnitFactory.Instance.GetAllActiveUnitsInEnemy(_unit.Team);

            _currentTarget = _detectData.Detect(_unit, units, _currentTarget);


            return _currentTarget;
        }
    }

    private void Awake()
    {
        _unit = GetComponent<Unit>();
    }
}

