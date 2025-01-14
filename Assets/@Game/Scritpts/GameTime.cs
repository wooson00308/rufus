using UnityEngine;

public class GameTime : SingletonMini<GameTime>
{
    public float timeScale;

    [Tooltip("게임 내 전체 속도 배수. 1이면 정상 속도, 0.5f면 절반 속도, 2이면 두 배속.")]
    public static float TimeScale { 
        get
        {
            return Instance.timeScale;
        }
        set
        {
            Instance.timeScale = value;
        }
    }

    public static float DeltaTime { get; private set; }

    public void FixedUpdate()
    {
        DeltaTime = Time.unscaledDeltaTime * timeScale;
    }
}
