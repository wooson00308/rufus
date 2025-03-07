using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class LobbyProcess : Process
{
    public PlayerHudView _playerHud;
    public SpawnConfig _playerSpawnConfig;
    public CinemachineCamera _camera;

    public List<SkillData> _skillDatas;

    private Unit _player;

    public void OnEnable()
    {
        GameEventSystem.Instance.Subscribe((int)InteractiveEvents.Portal_Enter, NextProcess);

        if(_player == null)
        {
            _player = UnitFactory.Instance.CreateUnit(_playerSpawnConfig.unit, _playerSpawnConfig.point.position, null, Team.Friendly, true);
            _player.Inventory.Equip(_playerSpawnConfig.item);
            _playerHud.SetPlayer(_player);
            _camera.Follow = _player.transform;

            foreach (var data in _skillDatas)
            {
                _player.ApplySkill(data);
                _player.ApplySkill(data);
            }
        }
    }

    public void OnDisable()
    {
        GameEventSystem.Instance.Unsubscribe((int)InteractiveEvents.Portal_Enter, NextProcess);
    }

    private void NextProcess(object gameEvent)
    {
        _processSystem.OnNextProcess<ReadyProcess>();
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
