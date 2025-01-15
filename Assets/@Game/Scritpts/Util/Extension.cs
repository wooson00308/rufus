using System;
using UnityEngine;
using UnityEngine.Events;

public static class Extension
{
    public static T GetOrAddComponent<T>(this GameObject go) where T : Component
    {
        return Util.GetOrAddComponent<T>(go);
    }
    public static void AddUniqueEventHandler(this UnityEvent unityEvent, UnityAction unityAction)
    {
        unityEvent.RemoveListener(unityAction);
        unityEvent.AddListener(unityAction);
    }

    public static byte SafeConvertToByte(this int value)
    {
        return SafeConvertToByte((float)value);
    }
    public static byte SafeConvertToByte(this float value)
    {
        const byte maxValue = byte.MaxValue; // 255
        const byte minValue = byte.MinValue; // 0

        return value switch
        {
            > maxValue => maxValue,
            < minValue => minValue,
            _ => (byte)value
        };
    }

    public static string Interpolation(this string value)
    {
        return value.Replace("{", "{{").Replace("}", "}}");
    }

    public static bool EqualsUnit(this Unit unit, Unit target)
    {
        if (unit == null || target == null) return false;
        return unit.GetInstanceID().Equals(target.GetInstanceID());
    }
}
