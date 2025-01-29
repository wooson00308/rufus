using System;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class LobbyProcess : Process
{
    public CinemachineCamera _camera;
    public List<SpawnConfig> _spawnConfig;

    public List<SkillData> _skillDatas;

    private Unit _player;

    public void OnEnable()
    {
        _player = UnitFactory.Instance.CreateUnit(_spawnConfig[0].unit, _spawnConfig[0].point.position, null, Team.Friendly, true);
        _camera.Follow = _player.transform;
        _player.Inventory.Equip(_spawnConfig[0].item);

        foreach (var data in _skillDatas)
        {
            _player.ApplySkill(data);
            _player.ApplySkill(data);
        }

        var enemys = UnitFactory.Instance.CreateUnits(_spawnConfig[1].unit, _spawnConfig[1].count, _spawnConfig[1].point.position, null, Team.Enemy);
        foreach (var enemy in enemys)
        {
            var item = _spawnConfig[1].item;
            if (item)
            {
                enemy.Inventory.Equip(_spawnConfig[1].item);
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
}
