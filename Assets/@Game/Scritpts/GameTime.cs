using UnityEngine;

public class GameTime : SingletonMini<GameTime>
{
    public float timeScale;

    [Tooltip("���� �� ��ü �ӵ� ���. 1�̸� ���� �ӵ�, 0.5f�� ���� �ӵ�, 2�̸� �� ���.")]
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
