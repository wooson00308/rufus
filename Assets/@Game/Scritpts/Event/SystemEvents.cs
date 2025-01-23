using UnityEngine;

public enum SystemEvents
{
    None = 100,
    CastingStart,
    Casting,
    CastingRemove,
    CastingInput,
    CastingEnd,
}

public enum CastingResultCode
{
    Success = 10,
    Error_FailedTyping,
    Error_CancelTyping,
}

public class SystemEventArgs
{

}

public class CastingSystemEventArgs : SystemEventArgs
{

}

public class CastingStartEventArgs: CastingSystemEventArgs
{
    
}

public class CastingInputEventArgs : CastingSystemEventArgs
{
    public string typedString;
}

public class CastingEndEventArgs: CastingSystemEventArgs
{
    public SkillData skillData;
    public bool isSuccess;
    public int resultCode;
}
