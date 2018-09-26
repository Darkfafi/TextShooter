using UnityEngine;

/// <summary>
/// The design is to have the View contain all the Unity / visual specific code and use the Model data to set its visual state.
/// </summary>
public abstract class MonoBaseView : MonoBehaviour, IView
{
    private BaseView _baseView = new BaseView();

    public Controller Controller
    {
        get
        {
            if (_baseView == null)
                return null;

            return _baseView.Controller;
        }
    }

    public virtual void DestroyView()
    {
        if(Controller == null)
        {
            return;
        }

        _baseView.DestroyView();
        OnViewDestroy();
        _baseView = null;
    }

    public virtual void SetupView(Controller controller)
    {
        _baseView.SetupView(controller);
        OnViewReady();
    }

    private void OnDestroy()
    {
        DestroyView();
    }

    protected virtual void OnViewReady() { }
    protected virtual void OnViewDestroy() { }
}

/// <summary>
/// The design is to have the View contain all the Unity / visual specific code and use the Model data to set its visual state.
/// </summary>
public class BaseView : IView
{
    public Controller Controller
    {
        get; private set;
    }

    private bool _internalDestroyed = false;

    public virtual void DestroyView()
    {
        if(!_internalDestroyed)
        {
            _internalDestroyed = true;
            Controller.Destroy();
            return;
        }

        if(Controller == null)
        {
            return;
        }

        OnViewDestroy();

        Controller = null;
    }

    public virtual void SetupView(Controller controller)
    {
        Controller = controller;
        OnViewReady();
    }

    private void OnDestroy()
    {
        OnViewDestroy();

        if (Controller != null)
        {
            Controller.Destroy();
        }
    }

    protected virtual void OnViewReady() { }
    protected virtual void OnViewDestroy() { }
}