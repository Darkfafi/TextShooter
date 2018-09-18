using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EntityModel : IModel
{
    public event Action<EntityModel> DestroyEvent;

    public Controller Controller
    {
        get; private set;
    }

    public virtual void Destroy()
    {
        if(DestroyEvent != null)
        {
            DestroyEvent(this);
        }
    }

    public void SetupModel(Controller controller)
    {
        Controller = controller;
    }
}
