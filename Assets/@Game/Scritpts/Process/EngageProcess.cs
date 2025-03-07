using System.Collections;
using System;
using UnityEngine;

public class EngageProcess : Process
{
    public Transform _spawnPoint;
    public SpawnConfig _spawnConfig;
    private bool _isLoopSpawnable;

    public override void SetActive(bool value)
    {
        base.SetActive(value);
        if(value)
        {
            var player = UnitFactory.Instance.GetAllActiveUnitsInTeam(Team.Friendly)[0];
            player.transform.position = _spawnPoint.position;
            StartCoroutine(ProcessLoopSpawn());
        }
    }

    public void OnDisable()
    {
        _isLoopSpawnable = false;
    }

    private IEnumerator ProcessLoopSpawn()
    {
        if (_isLoopSpawnable) yield break;
        _isLoopSpawnable = true;

        SpawnEnemy();

        while (_isLoopSpawnable)
        {
            float time = 0;

            while (time < _spawnConfig.delay)
            {
                time += GameTime.DeltaTime;
                yield return null;
            }

            SpawnEnemy();

            yield return null;
        }
    }

    private void SpawnEnemy()
    {
        var enemys = UnitFactory.Instance.CreateUnits(_spawnConfig.unit, _spawnConfig.count, _spawnConfig.point.position, null, Team.Enemy);
        foreach (var enemy in enemys)
        {
            var item = _spawnConfig.item;
            if (item)
            {
                enemy.Inventory.Equip(_spawnConfig.item);
            }
        }
    }
}


[Serializable]
public class SpawnConfig
{
    public Transform point;
    public UnitData unit;
    public ItemData item;
    public int count;
    public float delay;
}
