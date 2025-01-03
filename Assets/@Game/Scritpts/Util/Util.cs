using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Util
{
    const double EPSILON = 0.0001;

    public static T GetOrAddComponent<T>(GameObject go) where T : UnityEngine.Component
    {
        T component = go.GetComponent<T>();
        if (component == null)
            component = go.AddComponent<T>();

        return component;
    }

    public static GameObject FindChild(GameObject go, string name = null, bool recursive = false)
    {
        Transform transform = FindChild<Transform>(go, name, recursive);
        if (transform == null)
            return null;

        return transform.gameObject;
    }

    //최상위 오브젝트 , 찾을 이름, 재귀적으로 전체를 찾을것인지 
    public static T FindChild<T>(GameObject go, string name = null, bool recursive = false) where T : UnityEngine.Object
    {
        if (go == null)
            return null;
        if (recursive == false)
        {
            for (int i = 0; i < go.transform.childCount; i++)
            {
                Transform transform = go.transform.GetChild(i);
                if (string.IsNullOrEmpty(name) || transform.name == name)
                {
                    T component = transform.GetComponent<T>();
                    if (component != null)
                        return component;
                }
            }
        }
        else
        {
            foreach (T component in go.GetComponentsInChildren<T>())
            {
                if (string.IsNullOrEmpty(name) || component.name == name)
                    return component;
            }
        }

        return null;
    }

    public static T[] FindChildren<T>(GameObject go, string name = null, bool recursive = false)
        where T : UnityEngine.Object
    {
        if (go == null)
            return null;

        List<T> result = new List<T>();
        if (recursive == false)
        {
            for (int i = 0; i < go.transform.childCount; i++)
            {
                Transform transform = go.transform.GetChild(i);
                if (string.IsNullOrEmpty(name) || transform.name == name)
                {
                    T component = transform.GetComponent<T>();
                    if (component != null)
                        result.Add(component);
                }
            }
        }
        else
        {
            foreach (T component in go.GetComponentsInChildren<T>())
            {
                if (string.IsNullOrEmpty(name) || component.name == name)
                    result.Add(component);
            }
        }

        return result.ToArray();
    }

    public static bool IsEqual(double x, double y) // 비교 함수.
    {
        return (((x - EPSILON) < y) && (y < (x + EPSILON)));
    }

    public static string GetStringWithinSection(string str, string begin, string end)
    {
        if (string.IsNullOrEmpty(str))
        {
            return null;
        }

        string result = null;
        if (str.IndexOf(begin) > -1)
        {
            str = str.Substring(str.IndexOf(begin) + begin.Length);
            if (str.IndexOf(end) > -1) result = str.Substring(0, str.IndexOf(end));
            else result = str;
        }

        return result;
    }

    public static Vector3 WorldToCanvasPoint(Camera camera, Canvas canvas, Vector3 worldPosition)
    {
        // Vector3 result;
        Vector3 viewportPosition = camera.WorldToViewportPoint(worldPosition);

        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        var delta = canvasRect.sizeDelta;
        var result = new Vector2(
            ((viewportPosition.x * delta.x) - (delta.x * 0.5f)),
            ((viewportPosition.y * delta.y) - (delta.y * 0.5f)));


        return result;
    }

    /// <summary>
    /// 원형 범위에서 랜덤 위치 반환
    /// </summary>
    public static Vector2 GetRandomSpawnPositionCircle(Vector2 spawnPosition, float radius)
    {
        return spawnPosition + UnityEngine.Random.insideUnitCircle * radius;
    }

    /// <summary>
    /// 박스 범위에서 랜덤 위치 반환
    /// </summary>
    public static Vector2 GetRandomSpawnPositionBox(Vector2 spawnPosition, Vector2 boxSize)
    {
        float x = UnityEngine.Random.Range(-boxSize.x / 2, boxSize.x / 2);
        float y = UnityEngine.Random.Range(-boxSize.y / 2, boxSize.y / 2);
        return spawnPosition + new Vector2(x, y);
    }

    /// <summary>
    /// LookAt 2D버전 바라보는 각도 반환
    /// </summary>
    public static Vector3 LookAt2D(Transform transform, Transform targetPos)
    {
        Vector3 dir = targetPos.position - transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Vector3 lookDir = new Vector3(0, 0, angle);
        return lookDir;
    }

    public static Vector3 LookAt2D(Vector2 transform, Vector2 targetPos)
    {
        Vector3 dir = targetPos - transform;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Vector3 lookDir = new Vector3(0, 0, angle);
        return lookDir;
    }
}