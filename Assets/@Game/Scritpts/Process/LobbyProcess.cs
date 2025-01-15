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
        player.ApplySkill(_spawnConfig[0].skill);

        var enemys = UnitFactory.Instance.CreateUnits(_spawnConfig[1].unit, 100, _spawnConfig[1].point.position, null, Team.Enemy);
        foreach (var enemy in enemys)
        {
            var item = _spawnConfig[1].item;
            var skill = _spawnConfig[1].skill;
            if (item)
            {
                enemy.Inventory.Equip(_spawnConfig[1].item);
            }
            if (skill)
            {
                player.ApplySkill(_spawnConfig[1].skill);
            }
        }
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
    public SkillData skill;
}
