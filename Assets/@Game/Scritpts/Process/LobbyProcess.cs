using System;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class LobbyProcess : Process
{
    public CinemachineCamera _camera;
    public List<SpawnConfig> _spawnConfig;

    public void OnEnable()
    {
        var player = UnitFactory.Instance.CreateUnit(_spawnConfig[0].unit, _spawnConfig[0].point.position, null, Team.Friendly, true);
        _camera.Follow = player.transform;
        player.Inventory.Equip(_spawnConfig[0].item);
        var enemy = UnitFactory.Instance.CreateUnit(_spawnConfig[1].unit, _spawnConfig[1].point.position, null, Team.Enemy);
        enemy.Inventory.Equip(_spawnConfig[1].item);
    }

    public void OnDisable()
    {
        
    }
}


[Serializable]
public class SpawnConfig
{
    public Transform point;
    public UnitData unit;
    public ItemData item;
}
