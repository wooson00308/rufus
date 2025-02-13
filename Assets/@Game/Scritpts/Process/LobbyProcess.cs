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

    public bool OnTriggeredPortal { get; set; }

    public override void SetActive(bool value)
    {
        _env.SetActive(value);
        if (value)
        {
            if (_player != null && !_player.Status.IsDeath)
            {
                _player.ResetStats(Unit.ENGAGE_STATS_KEY);
            }
            else
            {
                _player = GameFactory.Instance.CreateUnit(_playerData, _spawnPoint.position, null, Team.Friendly, true);
                _playerHud.SetPlayer(_player);
                _camera.Follow = _player.transform;
                _player.Inventory.Equip(itemData);

                foreach (var data in _skillDatas)
                {
                    _player.ApplySkill(data);
                }
            }
        }
        else
        {
            OnTriggeredPortal = false;
        }

        base.SetActive(value);
    }

    public void Update()
    {
        if (!OnTriggeredPortal) return;

        if(Input.GetKeyDown(KeyCode.F))
        {
            _processSystem.OnNextProcess<ReadyProcess>();
        }
    }
}
