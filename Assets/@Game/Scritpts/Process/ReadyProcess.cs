using UnityEngine;

public class ReadyProcess : Process
{
    public Transform _spawnPoint;

    public GameObject _env;

    public bool OnTriggeredPortal { get; set; }

    public override void SetActive(bool value)
    {
        if(value)
        {
            var player = GameFactory.Instance.GetAllActiveUnitsInTeam(Team.Friendly)[0];
            player.transform.position = _spawnPoint.position;
        }

        _env.SetActive(value);
        base.SetActive(value);
    }

    public void Update()
    {
        if (!OnTriggeredPortal) return;

        if (Input.GetKeyDown(KeyCode.F))
        {
            _processSystem.OnNextProcess<EngageProcess>();
        }
    }
}
