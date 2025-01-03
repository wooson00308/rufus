using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitFactory : SingletonMini<UnitFactory>
{
    private readonly Dictionary<int, Unit> _activeUnits = new();

    /// <summary>
    /// 주어진 UnitData를 기반으로 유닛을 생성하거나 풀링하여 반환합니다.
    /// </summary>
    /// <param name="unitData">생성할 유닛의 데이터</param>
    /// <param name="position">유닛의 생성 위치</param>
    /// <param name="parent">유닛의 부모 Transform</param>
    /// <returns>생성된 Unit</returns>
    public Unit CreateUnit(UnitData unitData, Vector3 position, Transform parent = null, Team team = Team.Enemy)
    {
        if (unitData == null)
        {
            Debug.LogError("UnitData가 null입니다.");
            return null;
        }

        GameObject unitObject = ResourceManager.Instance.Spawn(unitData.Prefab.gameObject, parent);
        unitObject.transform.position = position;

        Unit unit = unitObject.GetComponent<Unit>();
        if (unit == null)
        {
            Debug.LogError("Unit prefab에 Unit 컴포넌트가 없습니다.");
            return null;
        }

        unit.Initialized(unitData, team);
        unit.gameObject.name = unit.GetInstanceID().ToString();

        _activeUnits[unit.GetInstanceID()] = unit;

        return unit;
    }

    /// <summary>
    /// 주어진 UnitData를 기반으로 여러 유닛을 생성하거나 풀링하여 반환합니다.
    /// </summary>
    /// <param name="unitData">생성할 유닛의 데이터</param>
    /// <param name="count">생성할 유닛의 수</param>
    /// <param name="startPosition">유닛들의 시작 위치</param>
    /// <param name="parent">유닛들의 부모 Transform</param>
    /// <param name="team">유닛이 속할 팀</param>
    /// <returns>생성된 Unit 목록</returns>
    public List<Unit> CreateUnits(UnitData unitData, int count, Vector3 startPosition, Transform parent = null, Team team = Team.Enemy)
    {
        if (unitData == null)
        {
            Debug.LogError("UnitData가 null입니다.");
            return null;
        }

        List<Unit> createdUnits = new List<Unit>();

        for (int i = 0; i < count; i++)
        {
            // 예: x 방향으로 약간씩 떨어뜨려 스폰
            Vector3 spawnPos = startPosition + new Vector3(i * 2f, 0f, 0f);

            // 기존의 CreateUnit 메서드를 재활용합니다.
            Unit unit = CreateUnit(unitData, spawnPos, parent, team);
            if (unit != null)
            {
                createdUnits.Add(unit);
            }
        }

        return createdUnits;
    }


    /// <summary>
    /// 유닛을 파괴(풀에 반환)합니다.
    /// </summary>
    /// <param name="unit">unit.GetInstanceID()</param>
    public void DestroyUnit(int id)
    {
        if (_activeUnits.TryGetValue(id, out Unit unit))
        {
            _activeUnits.Remove(id);
        }

        ResourceManager.Instance.Destroy(unit.gameObject);
    }

    /// <summary>
    /// 현재 활성화된 모든 유닛을 반환합니다.
    /// </summary>
    /// <returns>활성화된 유닛의 리스트</returns>
    public List<Unit> GetAllActiveUnits()
    {
        return new List<Unit>(_activeUnits.Values);
    }

    public List<Unit> GetAllActiveUnitsInTeam(Team team)
    {
        return GetAllActiveUnits().FindAll(x => x.Team == team);
    }

    public List<Unit> GetAllActiveUnitsInEnemy(Team team)
    {
        return GetAllActiveUnits().FindAll(unit => unit.Team != team);
    }

    public List<Unit> GetAllActiveUnitsInAoERadius(Unit target, List<Unit> units, float radius)
    {
        List<Unit> unitsInRadius = new() { target };
        Vector3 unitPosition = target.transform.position;

        foreach (Unit otherUnit in units)
        {
            if (otherUnit == target)
                continue;

            if (!otherUnit.IsActive)
                continue;

            float distance = Vector3.Distance(unitPosition, otherUnit.transform.position);

            if (distance <= radius)
            {
                unitsInRadius.Add(otherUnit);
            }
        }

        return unitsInRadius;
    }


    /// <summary>
    /// 특정 ID의 유닛을 반환합니다.
    /// </summary>
    /// <param name="id">찾을 유닛의 ID</param>
    /// <returns>찾은 유닛 또는 null</returns>
    public Unit GetUnitById(int id)
    {
        _activeUnits.TryGetValue(id, out var unit);
        return unit;
    }

    /// <summary>
    /// 모든 활성화된 유닛을 제거하고 풀에 반환합니다.
    /// </summary>
    public void ClearAllUnits()
    {
        foreach (var unit in _activeUnits.Values)
        {
            ResourceManager.Instance.Destroy(unit.gameObject);
        }

        _activeUnits.Clear();
    }
}

public enum Team
{
    Friendly,
    Enemy
}