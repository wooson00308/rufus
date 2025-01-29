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
    public string typedString;
    public string castString;
    public int level = 0;
}

public class CastingStartEventArgs: CastingSystemEventArgs
{
    
}

public class CastingInputEventArgs : CastingSystemEventArgs
{
    public string keyString;
    public bool isTypo;
}

public class CastingEndEventArgs: CastingSystemEventArgs
{
    public SkillData skillData;
    public int resultCode;
    public bool isSuccess;
}
