using System;

/// <summary>
/// The design is to have the model contain no Unity specifications. It serves as a data object for its respective view.
/// </summary>
public abstract class BaseModel : IModel, IComponentsHolder
{
    public event Action<BaseModel> DestroyEvent;

    public bool IsDestroyed
    {
        get; private set;
    }

    public IAbstractController LinkingController
    {
        get; private set;
    }

    public MethodPermitter MethodPermitter
    {
        get; private set;
    }

    private ModelComponents _components;
    private bool _internalDestroyCalled = false;

    public BaseModel()
    {
        MethodPermitter = new MethodPermitter();
        _components = new ModelComponents(this);
    }

    public void Destroy()
    {
        if(IsDestroyed)
        {
            return;
        }

        if(!_internalDestroyCalled)
        {
            _internalDestroyCalled = true;
            LinkingController.Destroy();
            return;
        }

        IsDestroyed = true;

        if (DestroyEvent != null)
        {
            DestroyEvent(this);
        }

        OnModelDestroy();

        _components.Clean();
        _components = null;

        LinkingController = null;
    }

    public void SetupModel(IAbstractController controller)
    {
        if (LinkingController != null)
            return;

        LinkingController = controller;
        IsDestroyed = false;
        _components.SignalReady();
        OnModelReady();
    }

    protected virtual void OnModelReady() { }
    protected virtual void OnModelDestroy() { }

    public T AddComponent<T>() where T : BaseModelComponent
    {
        return _components.AddComponent<T>();
    }

    public void RemoveComponent<T>() where T : BaseModelComponent
    {
        _components.RemoveComponent<T>();
    }

    public T GetComponent<T>() where T : BaseModelComponent
    {
        return _components.GetComponent<T>();
    }
}
