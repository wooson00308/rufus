using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    private int _order = 10;
    private Stack<IPopup> _popupStack = new Stack<IPopup>();

    private Dictionary<string, IBasePresenter<BaseView, BaseModel>> _layoutPresenters = new Dictionary<string, IBasePresenter<BaseView, BaseModel>>();
    public IPopup PeekPopup => _popupStack.Count > 0 ? _popupStack.Peek() : null;

    private (Canvas canvas, CanvasScaler scaler, GraphicRaycaster raycaster) _root;
    public (Canvas canvas, CanvasScaler scaler, GraphicRaycaster raycaster) Root
    {
        get
        {
            if (_root == (null, null, null))
            {
                _root = CreateUIRoot();
            }
            return _root;
        }
    }

    public void SetCanvas(GameObject go, bool sort = true, RenderMode mode = RenderMode.ScreenSpaceCamera)
    {
        var canvas = go.GetOrAddComponent<Canvas>();
        ConfigureCanvas(canvas, sort, mode);
    }

    public Canvas GetCanvas(GameObject go) => go.GetOrAddComponent<Canvas>();

    public T ShowLayoutUI<T>(string name = null) where T : Component, IBasePresenter<BaseView, BaseModel>
    {
        name = string.IsNullOrEmpty(name) ? typeof(T).Name : name;
        if (_layoutPresenters.TryGetValue(name, out var presenter))
            return (T)presenter;

        presenter = InstantiateLayout<T>(name);
        Configure(presenter);
        _layoutPresenters.Add(name, presenter);
        return (T)presenter;
    }

    public void CloseLayoutUI<T>(string name = null) where T : Component, IBasePresenter<BaseView, BaseModel>
    {
        name = string.IsNullOrEmpty(name) ? typeof(T).Name : name;
        if (!_layoutPresenters.TryGetValue(name, out var presenter)) return;

        _layoutPresenters.Remove(name);
        var layout = (T)presenter;
        ResourceManager.Instance.DestroyUI(layout.gameObject);
    }

    private void Configure<T>(T presenter) where T : IBasePresenter<BaseView, BaseModel>
    {
        presenter.Tr.SetParent(Root.canvas.transform, false);
    }

    public T GetPopupUI<T>(string name = null) where T : Component, IPopup
    {
        name = string.IsNullOrEmpty(name) ? typeof(T).Name : name;
        var popup = InstantiatePopup<T>(name);
        ConfigurePopup(popup);
        return popup;
    }

    public void ClosePopupUI<T>(T popup) where T : Component, IPopup
    {
        if (IsPopupValidToClose(popup))
        {
            ClosePopupUI<T>();
        }
    }

    private void ClosePopupUI<T>() where T : Component, IPopup
    {
        if (_popupStack.Count <= 0) return;

        var popup = _popupStack.Pop() as T;
        _order--;
        ResourceManager.Instance.DestroyUI(popup.gameObject);
    }

    private (Canvas canvas, CanvasScaler scaler, GraphicRaycaster raycaster) CreateUIRoot()
    {
        GameObject go = GameObject.Find("@UI_Root") ?? new GameObject("@UI_Root");
        var canvas = go.GetOrAddComponent<Canvas>();
        ConfigureCanvasSettings(canvas);
        var scaler = go.GetOrAddComponent<CanvasScaler>();
        ConfigureScalerSettings(scaler);
        var raycaster = go.GetOrAddComponent<GraphicRaycaster>();
        return (canvas, scaler, raycaster);
    }

    private void ConfigureCanvas(Canvas canvas, bool sort, RenderMode mode)
    {
        canvas.renderMode = mode;
        canvas.worldCamera = Camera.main;
        canvas.overrideSorting = true;
        canvas.sortingOrder = sort ? _order++ : 0;
    }

    private void ConfigurePopup<T>(T popup) where T : Component, IPopup
    {
        _popupStack.Push(popup);
        popup.tr.SetParent(Root.canvas.transform, false);
        popup.Initialize();
    }

    private bool IsPopupValidToClose<T>(T popup) where T : Component, IPopup
    {
        if (_popupStack.Count == 0 || _popupStack.Peek() != popup)
        {
            Debug.LogWarning($"Close Popup Failed! Peek: {_popupStack.Peek()} \nInput Popup: {popup}");
            return false;
        }
        return true;
    }

    private static void ConfigureCanvasSettings(Canvas canvas)
    {
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
    }

    private static void ConfigureScalerSettings(CanvasScaler scaler)
    {
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080, 1920);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
    }

    private T InstantiatePopup<T>(string name) where T : Component, IPopup
    {
        return ResourceManager.Instance.SpawnFromPath($"UI/Popup/{name}").GetComponent<T>();
    }

    private T InstantiateLayout<T>(string name) where T : Component, IBasePresenter<BaseView, BaseModel>
    {
        return ResourceManager.Instance.SpawnFromPath($"UI/Layout/{name}").GetComponent<T>();
    }
}
