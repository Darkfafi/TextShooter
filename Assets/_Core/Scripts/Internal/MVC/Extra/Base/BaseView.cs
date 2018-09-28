using System;
using UnityEngine;

/// <summary>
/// The design is to have the View contain all the Unity / visual specific code and use the Model data to set its visual state.
/// It is to only read from the Model, not to alter its data.
/// </summary>
public abstract class MonoBaseView : MonoBehaviour, IView
{
    private BaseView _baseView = new BaseView();

    public object LinkingController
    {
        get
        {
            if (_baseView == null)
                return null;

            return _baseView.LinkingController;
        }
    }

    public MethodPermitter MethodPermitter
    {
        get
        {
            if (_baseView == null)
                return null;

            return _baseView.MethodPermitter;
        }
    }

    public virtual void DestroyView()
    {
        if(LinkingController == null)
        {
            return;
        }

        _baseView.DestroyView();
        OnViewDestroy();
        ViewDestruction();
        _baseView = null;
    }

    public virtual void SetupView(object controller)
    {
        if (LinkingController != null)
            return;

        _baseView.SetupView(controller);
        OnViewReady();
    }

    protected virtual void ViewDestruction()
    {
        Destroy(gameObject);
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
    public object LinkingController
    {
        get; private set;
    }

    public MethodPermitter MethodPermitter
    {
        get; private set;
    }

    public virtual void DestroyView()
    {
        if(LinkingController == null)
        {
            return;
        }

        OnViewDestroy();

        LinkingController = null;
    }

    public virtual void SetupView(object controller)
    {
        if (LinkingController != null)
            return;

        LinkingController = controller;
        MethodPermitter = MVCUtil.GetModel<BaseModel>(this).MethodPermitter;
        OnViewReady();
    }

    protected virtual void OnViewReady() { }
    protected virtual void OnViewDestroy() { }
}