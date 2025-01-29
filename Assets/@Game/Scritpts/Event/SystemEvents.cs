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
    public string typedString;
    public string castingString;
}

public class CastingInputEventArgs : CastingSystemEventArgs
{
    public bool isTypo;
    public string typedString;
    public string castingString;
}

public class CastingEndEventArgs: CastingSystemEventArgs
{
    public SkillData skillData;
    public int level = 0;
    public int resultCode;
    public bool isSuccess;
}
