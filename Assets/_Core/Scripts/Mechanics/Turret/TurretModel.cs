using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretModel : EntityModel
{
    public delegate void NewOldTargetHandler(EntityModel newTarget, EntityModel previousTarget);
    public event NewOldTargetHandler TargetSetEvent;

    public EntityModel CurrentTarget { get; private set; }

    public void FocusOnTarget(EntityModel target)
    {
        EntityModel previousTarget = CurrentTarget;
        CurrentTarget = target;

        if(TargetSetEvent != null)
        {
            TargetSetEvent(CurrentTarget, previousTarget);
        }
    }
}
