using UnityEngine;

public class ReadyProcess : Process
{
    public Transform _spawnPoint;

    public GameObject _env;

    public override void SetActive(bool value)
    {
        if(value)
        {
            var player = UnitFactory.Instance.GetAllActiveUnitsInTeam(Team.Friendly)[0];
            player.transform.position = _spawnPoint.position;
        }

        _env.SetActive(value);
        base.SetActive(value);
    }
}
