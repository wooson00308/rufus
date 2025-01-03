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
        var player = UnitFactory.Instance.CreateUnit(_spawnConfig[0].unit, _spawnConfig[0].point.position, null, Team.Friendly);
        _camera.Follow = player.transform;
        UnitFactory.Instance.CreateUnit(_spawnConfig[1].unit, _spawnConfig[1].point.position, null, Team.Enemy);
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
}
