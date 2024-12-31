using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DistanceDetectData", menuName = "Data/Unit/Detect/Create DistanceDetectData")]
public class DistanceDetectData : DetectDataBase
{
    [SerializeField]
    private bool _isClosestDistance = true;

    public override Unit Detect(Unit user, List<Unit> enemies, Unit currentTarget = null)
    {
        if (_isLockOnTargetUntilDeath && currentTarget != null && !currentTarget.Status.IsDeath)
        {
            return currentTarget;
        }

        Unit bestTarget = null;
        float bestDistance = _isClosestDistance ? float.MaxValue : float.MinValue;

        foreach (Unit unit in enemies)
        {
            if (unit.Status.IsDeath || unit == user || !unit.gameObject.activeSelf)
                continue;

            float unitDistance = Vector3.Distance(user.transform.position, unit.transform.position);

            bool isBetterTarget = _isClosestDistance ? unitDistance < bestDistance : unitDistance > bestDistance;

            if (isBetterTarget)
            {
                bestDistance = unitDistance;
                bestTarget = unit;
            }
        }

        return bestTarget;
    }
}
