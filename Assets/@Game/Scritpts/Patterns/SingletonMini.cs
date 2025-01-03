using UnityEngine;

/// <summary>
/// 하나하나 주입시키기 번거로우면 사용할 것 (남용 금지)
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class SingletonMini<T> : MonoBehaviour where T : SingletonMini<T>
{
    private static T _instance;
    public static T Instance => _instance;


    protected virtual void Awake()
    {
        _instance = FindAnyObjectByType<T>();
    }

    protected virtual void OnDestroy()
    {
        Destroy(_instance);
    }

}
