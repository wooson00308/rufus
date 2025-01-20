using UnityEngine;

public enum SystemEvents
{
    None = 100,
    CasingStart,
    Casing,
    CasingEnd,
}

public enum CastingFailedTypes
{
    None = 10,
    FailedTyping,
    CancelTyping,
}

public class SystemEventArgs
{

}

public class CastingSystemEventArgs : SystemEventArgs
{
    public string answer;
    public string typedString;
}

public class CastingStartEventArgs: CastingSystemEventArgs
{
    
}

public class CastingInputEventArgs : CastingSystemEventArgs
{

}

public class CastingEndEventArgs: CastingSystemEventArgs
{
    public bool isSuccess;
    public int failedCode;
}
