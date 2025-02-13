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
            GameEventSystem.Instance.Subscribe((int)UnitEvents.Death, OnPlayerDeath);
            var player = GameFactory.Instance.GetAllActiveUnitsInTeam(Team.Friendly)[0];
            player.transform.position = _spawnPoint.position;
            StartCoroutine(ProcessLoopSpawn());
        }
        else
        {
            GameFactory.Instance.ClearAllUnits();
            GameFactory.Instance.ClearAllProjectiles();
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

    private void OnPlayerDeath(object gameEvent)
    {
        var args = gameEvent as UnitEventArgs;
        if(args.publisher.IsPlayer)
        {
            _processSystem.OnNextProcess<LobbyProcess>();
        }
    }

    private void SpawnEnemy()
    {
        var enemys = GameFactory.Instance.CreateUnits(_spawnConfig.unit, _spawnConfig.count, _spawnConfig.point.position, null, Team.Enemy);
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
