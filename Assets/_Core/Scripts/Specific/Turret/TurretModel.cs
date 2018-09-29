using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretModel : EntityModel
{
    public delegate void NewOldTargetHandler(EntityModel newTarget, EntityModel previousTarget);
    public event NewOldTargetHandler TargetSetEvent;

    public EntityModel CurrentTarget { get; private set; }
    public float TurretNeckRotation { get; private set; }

    private TimekeeperModel _timekeeper;

    public TurretModel(TimekeeperModel timekeeper)
    {
        _timekeeper = timekeeper;
        _timekeeper.ListenToFrameTick(Update);
    }

    protected override void OnModelDestroy()
    {
        base.OnModelDestroy();
        _timekeeper.UnlistenFromFrameTick(Update);
        _timekeeper = null;
    }

    private void Update(float deltaTime, float timeScale)
    {
        if (CurrentTarget == null)
        {
            return;
        }

        float x = CurrentTarget.ModelTransform.Position.x - ModelTransform.Position.x;
        float y = CurrentTarget.ModelTransform.Position.y - ModelTransform.Position.y;

        float angle = (Mathf.Atan2(y, x) * Mathf.Rad2Deg) - 90f;

        float newAngle = Mathf.LerpAngle(TurretNeckRotation, angle, deltaTime * timeScale * 7.4f);

        TurretNeckRotation = newAngle;
    }

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
