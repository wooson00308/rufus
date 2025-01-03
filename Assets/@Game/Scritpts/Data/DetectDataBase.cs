using System.Collections.Generic;
using UnityEngine;

public abstract class DetectDataBase : Data
{
    public bool _isLockOnTargetUntilDeath;

    public abstract Unit Detect(Unit user, List<Unit> enemies, Unit currentTarget = null);
}
