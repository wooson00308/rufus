using UnityEngine;

/// <summary>
/// 싱글톤 사용할 때 상속받아서 쓰세요
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    private static T _instance;
    public static T Instance
    {
        get
        {
            CreateInstance();
            return _instance;
        }
    }

    private static void CreateInstance()
    {
        if (_instance == null)
        {
            //find existing instance
            _instance = FindAnyObjectByType<T>();
            if (_instance == null)
            {
                //create new instance
                var name = typeof(T).Name;
                if (Resources.Load<T>($"Prefabs/{name}") is var prefab && prefab != null)
                {
                    _instance = Instantiate(prefab, Vector3.zero, Quaternion.identity);
                    _instance.name = name;
                }
                else
                {
                    var go = new GameObject(name);
                    _instance = go.AddComponent<T>();
                }

            }
            //initialize instance if necessary
            if (!_instance._initialized)
            {
                _instance.Initialize();
                _instance._initialized = true;
            }
        }
    }

    protected virtual void Awake()
    {
        if (_instance != null && _instance != this)
        {
            DestroyImmediate(gameObject);
            return;
        }

        _instance = (T)this;

        if (Application.isPlaying)
        {
            DontDestroyOnLoad(this);
        }
    }

    private bool _initialized;

    protected virtual void Initialize() { }
}
