using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base Model for the game. Recommended to be used with the EntityManager, but not required.
/// The design is to have the model contain no Unity specifications. It serves as a data object for its respective view.
/// </summary>
public abstract class BaseModel : IModel
{
    public event Action<BaseModel> DestroyEvent;

    public bool IsDestroyed
    {
        get; private set;
    }

    public Controller Controller
    {
        get; private set;
    }

    private bool _internalDestroyCalled = false;

    public void Destroy()
    {
        if(IsDestroyed)
        {
            return;
        }

        if(!_internalDestroyCalled)
        {
            _internalDestroyCalled = true;
            Controller.Destroy();
            return;
        }

        IsDestroyed = true;

        OnEntityDestroy();
        if (DestroyEvent != null)
        {
            DestroyEvent(this);
        }

        Controller = null;
    }

    public void SetupModel(Controller controller)
    {
        Controller = controller;
        IsDestroyed = false;
        OnEntityReady();
    }

    protected virtual void OnEntityReady() { }
    protected virtual void OnEntityDestroy() { }
}
