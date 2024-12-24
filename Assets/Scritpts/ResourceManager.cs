using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class ResourceManager : Singleton<ResourceManager>
{
    // 오브젝트 풀을 관리하기 위한 딕셔너리
    private readonly Dictionary<string, Queue<GameObject>> _objectPool = new();

    /// <summary>
    /// 프리팹을 풀링해서 오브젝트를 반환함
    /// </summary>
    /// <param name="prefab"></param>
    /// <param name="parent"></param>
    /// <returns></returns>
    public GameObject Spawn(GameObject prefab, Transform parent = null)
    {
        string key = prefab.name;

        // 만약 풀에 해당 키가 없다면, 새로 생성
        if (!_objectPool.ContainsKey(key))
        {
            _objectPool[key] = new Queue<GameObject>();
        }

        // 풀에 오브젝트가 존재하는지 확인
        if (_objectPool[key].Count > 0)
        {
            // 큐에서 오브젝트를 가져옴
            GameObject pooledObject = _objectPool[key].Dequeue();

            pooledObject.SetActive(true);
            if (parent != null) pooledObject.transform.SetParent(parent);
            ResetObject(pooledObject);
            return pooledObject;
        }
        else
        {
            // 풀에 사용 가능한 오브젝트가 없다면 새로 생성
            GameObject newObject = Instantiate(prefab, parent);
            newObject.name = key;
            return newObject;
        }
    }

    /// <summary>
    /// 경로에서 리소스를 불러와서 풀링시키고 오브젝트를 반환함
    /// </summary>
    /// <param name="path"></param>
    /// <param name="parent"></param>
    /// <returns></returns>
    public GameObject SpawnFromPath(string path, Transform parent = null)
    {
        // 리소스를 경로에서 로드
        GameObject prefab = Resources.Load<GameObject>(path);
        if (prefab == null)
        {
            Debug.LogError($"리소스 경로 '{path}'에서 프리팹을 찾을 수 없습니다.");
            return null;
        }

        // 기존 Spawn 메서드를 사용해 프리팹을 풀링해서 가져오기
        return Spawn(prefab, parent);
    }

    /// <summary>
    /// 오브젝트를 풀 안에 반환시킴
    /// </summary>
    /// <param name="obj"></param>
    public void Destroy(GameObject obj)
    {
        if (!obj.activeInHierarchy)
        {
            Debug.LogWarning("이미 풀에 반환된 오브젝트입니다.");
            return;
        }

        obj.SetActive(false);
        obj.transform.SetParent(transform);  // 풀 관리용 부모로 설정

        if (!_objectPool.ContainsKey(obj.name))
        {
            _objectPool[obj.name] = new Queue<GameObject>();
        }

        // 큐에 추가
        _objectPool[obj.name].Enqueue(obj);
    }

    /// <summary>
    /// UI 오브젝트를 풀에 반환시키고, 자식에 새로운 Canvas를 생성한 뒤 부모로 할당함
    /// </summary>
    /// <param name="uiObject"></param>
    public void DestroyUI(GameObject uiObject)
    {
        if (!uiObject.activeInHierarchy)
        {
            //Debug.LogWarning("이미 풀에 반환된 UI 오브젝트입니다.");
            return;
        }

        var canvasChild = transform.Find("UI_Canvas");
        GameObject canvasObject = canvasChild?.gameObject;

        if (canvasObject == null)
        {
            canvasObject = new GameObject("UI_Canvas");

            Canvas canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            var scaler = canvasObject.AddComponent<CanvasScaler>(); // UI 스케일 조절
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080, 1920);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;

            canvasObject.AddComponent<GraphicRaycaster>(); // UI 클릭 이벤트 처리

            canvasObject.transform.SetParent(transform, false);
        }
        
        // UI 부모를 ResourceManager로 설정
        uiObject.transform.SetParent(canvasObject.transform, false);

        // UI 오브젝트를 비활성화하고 풀에 추가
        uiObject.SetActive(false);

        if (!_objectPool.ContainsKey(uiObject.name))
        {
            _objectPool[uiObject.name] = new Queue<GameObject>();
        }

        _objectPool[uiObject.name].Enqueue(uiObject);
    }


    public Queue<GameObject> GetPoolObjects(string key)
    {
        return _objectPool[key];
    }

    /// <summary>
    /// 오브젝트 상태 초기화
    /// </summary>
    /// <param name="obj"></param>
    private void ResetObject(GameObject obj)
    {
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localRotation = Quaternion.identity;
    }
}
