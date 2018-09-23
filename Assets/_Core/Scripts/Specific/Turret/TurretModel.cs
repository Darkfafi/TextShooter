using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretModel : BaseModel
{
    public delegate void NewOldTargetHandler(BaseModel newTarget, BaseModel previousTarget);
    public event NewOldTargetHandler TargetSetEvent;

    public BaseModel CurrentTarget { get; private set; }

    public void FocusOnTarget(BaseModel target)
    {
        BaseModel previousTarget = CurrentTarget;
        CurrentTarget = target;

        if(TargetSetEvent != null)
        {
            TargetSetEvent(CurrentTarget, previousTarget);
        }
    }
}
