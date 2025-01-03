using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseView : MonoBehaviour
{
    protected Dictionary<Type, UnityEngine.Object[]> _objects = new Dictionary<Type, UnityEngine.Object[]>();
    protected void Bind<T>(Type type) where T : UnityEngine.Object
    {
        Type key = typeof(T);
        string[] names = Enum.GetNames(type);
        UnityEngine.Object[] objects = new UnityEngine.Object[names.Length];
        _objects.Add(key, objects);

        for (int i = 0; i < names.Length; i++)
        {
            if (key == typeof(GameObject))
                objects[i] = Util.FindChild(gameObject, names[i], true);
            else
                objects[i] = Util.FindChild<T>(gameObject, names[i], true);

            if (objects[i] == null)
                Debug.Log($"Failed to Bind({names[i]})");
        }
    }

    public T Get<T>(int idx) where T : UnityEngine.Object
    {
        UnityEngine.Object[] objects = null;
        if (_objects.TryGetValue(typeof(T), out objects) == false)
            return null;

        return objects[idx] as T;

    }

    public abstract void BindUI();
}