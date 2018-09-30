using System;

/// <summary>
/// The design is to have the model contain no Unity specifications. It serves as a data object for its respective view.
/// </summary>
public abstract class BaseModel : IModel
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
        get
        {
            return _methodPermitter;
        }
    }

    private bool _internalDestroyCalled = false;
    private MethodPermitter _methodPermitter = new MethodPermitter();

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

        LinkingController = null;
    }

    public void SetupModel(IAbstractController controller)
    {
        if (LinkingController != null)
            return;

        LinkingController = controller;
        IsDestroyed = false;
        OnModelReady();
    }

    protected virtual void OnModelReady() { }
    protected virtual void OnModelDestroy() { }
}
