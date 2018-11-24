using System;
using UnityEngine;
using System.Collections.Generic;


public class TurretModel : EntityModel
{
    public delegate void NewOldTargetHandler(EntityModel newTarget, EntityModel previousTarget);
    public event NewOldTargetHandler TargetSetEvent;

    public EnemyModel CurrentTarget { get; private set; }
    public float TurretNeckRotation { get; private set; }
    public float Range { get; private set; }

    private TimekeeperModel _timekeeper;

    private EntityFilter<EnemyModel> _enemyFilter = EntityFilter<EnemyModel>.Create(Tags.ENEMY);

    public TurretModel(TimekeeperModel timekeeper)
    {
        _timekeeper = timekeeper;
        _timekeeper.ListenToFrameTick(Update);
        Range = 5f;
    }

    protected override void OnModelDestroy()
    {
        base.OnModelDestroy();

        if(CurrentTarget != null)
        {
            FocusOnTarget(null);
        }

        _timekeeper.UnlistenFromFrameTick(Update);
        _timekeeper = null;

        _enemyFilter.Clean();
        _enemyFilter = null;
    }

    private void Update(float deltaTime, float timeScale)
    {  
        EnemyModel target = _enemyFilter.GetFirst(
            (e) =>
            {
                if (e.IsDestroyed || e.IsDead)
                    return false;

                if ((e.ModelTransform.Position - ModelTransform.Position).magnitude > Range)
                    return false;

                return true;

            }, (a, b) =>
            {
                float distA = (a.ModelTransform.Position - ModelTransform.Position).magnitude;
                float distB = (b.ModelTransform.Position - ModelTransform.Position).magnitude;
                return (int)(distA - distB);
            });

        FocusOnTarget(target);

        float angleToTarget = 0;

        if (CurrentTarget != null)
        {
            float x = CurrentTarget.ModelTransform.Position.x - ModelTransform.Position.x;
            float y = CurrentTarget.ModelTransform.Position.y - ModelTransform.Position.y;

            angleToTarget = (Mathf.Atan2(y, x) * Mathf.Rad2Deg) - 90f;
        }

        TurretNeckRotation = Mathf.LerpAngle(TurretNeckRotation, angleToTarget, deltaTime * timeScale * 7.4f);
    }

    public void FocusOnTarget(EnemyModel target)
    {
        EnemyModel previousTarget = CurrentTarget;

        if(previousTarget == target)
            return;

        CurrentTarget = target;

        if (TargetSetEvent != null)
        {
            TargetSetEvent(CurrentTarget, previousTarget);
        }
    }
}
