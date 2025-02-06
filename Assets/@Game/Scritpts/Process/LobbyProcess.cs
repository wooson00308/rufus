using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class LobbyProcess : Process
{
    public GameObject _env;

    public UnitData _playerData;
    public ItemData itemData;
    public List<SkillData> _skillDatas;
    
    public Transform _spawnPoint;
    public PlayerHudView _playerHud;
    public CinemachineCamera _camera;


    private Unit _player;

    public override void SetActive(bool value)
    {
        _env.SetActive(value);
        if (value)
        {
            if (_player != null)
            {
                _player.ResetStats(Unit.ENGAGE_STATS_KEY);
            }
            else
            {
                _player = UnitFactory.Instance.CreateUnit(_playerData, _spawnPoint.position, null, Team.Friendly, true);
                _playerHud.SetPlayer(_player);
                _camera.Follow = _player.transform;
                _player.Inventory.Equip(itemData);

                foreach (var data in _skillDatas)
                {
                    _player.ApplySkill(data);
                }
            }
        }

        base.SetActive(value);
    }

    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            _processSystem.OnNextProcess<ReadyProcess>();
        }
    }
}
