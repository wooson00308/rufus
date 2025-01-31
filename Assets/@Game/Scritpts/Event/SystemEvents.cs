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
    public SkillData skillData;
    public string typedString;
    public string castString;
    public int succesLevel = 0;
    public int castLevel = 0;
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
    public int resultCode;
    public bool isSuccess;
}
