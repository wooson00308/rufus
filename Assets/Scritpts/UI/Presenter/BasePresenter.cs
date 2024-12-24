using UnityEngine;

public interface IBasePresenter<out TView, out TModel>
    where TView : BaseView
    where TModel : BaseModel
{
    public TView View { get; }
    public TModel Model { get; }

    public Transform Tr { get; }
}

public abstract class BasePresenter<V, M> : MonoBehaviour, IBasePresenter<V, M>
    where V : BaseView
    where M : BaseModel
{
    public Transform Tr => transform;
    public V View => _view;
    public M Model => _model;

    [SerializeField] protected V _view;
    [SerializeField] protected M _model;
}