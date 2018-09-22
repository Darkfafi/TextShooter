using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base Model for the game. Recommended to be used with the EntityManager, but not required.
/// The design is to have the model contain no Unity specifications. It serves as a data object for its respective view.
/// </summary>
public abstract class EntityModel : IModel
{
    public event Action<EntityModel> DestroyEvent;

    public bool IsDestroyed
    {
        get; private set;
    }

    public Controller Controller
    {
        get; private set;
    }

    public void Destroy()
    {
        OnEntityDestroy();
        IsDestroyed = true;
        if (DestroyEvent != null)
        {
            DestroyEvent(this);
        }
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
