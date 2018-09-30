using System;
using UnityEngine;

public class TurretModel : EntityModel
{
    public delegate void NewOldTargetHandler(EntityModel newTarget, EntityModel previousTarget);
    public event NewOldTargetHandler TargetSetEvent;

    public EnemyModel CurrentTarget { get; private set; }
    public float TurretNeckRotation { get; private set; }
    public float Range { get; private set; }

    private TimekeeperModel _timekeeper;

    private EntityFilter<EnemyModel> _enemyFilter = new EntityFilter<EnemyModel>(TagFilterType.HasAnyTag, "Enemy");

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
        float distCurrent = CurrentTarget == null ? float.MaxValue : (CurrentTarget.ModelTransform.Position - ModelTransform.Position).magnitude;

        EnemyModel otherTarget = _enemyFilter.GetAny(
            (e) =>
            {
                float dist = (e.ModelTransform.Position - ModelTransform.Position).magnitude;
                if (dist > Range)
                {
                    return false;
                }

                if (dist < distCurrent)
                {
                    return true;
                }

                return false;
            });


        if (otherTarget == null)
        {
            if(distCurrent > Range)
                FocusOnTarget(null);
        }
        else if ((otherTarget.ModelTransform.Position - ModelTransform.Position).magnitude < distCurrent)
        {
            FocusOnTarget(otherTarget);
        }

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

    public void FocusOnTarget(EnemyModel target)
    {
        EnemyModel previousTarget = CurrentTarget;

        if(previousTarget == target)
            return;

        if(previousTarget != null)
        {
            previousTarget.DestroyEvent -= OnDestroyEvent;
            previousTarget.DeathEvent -= OnDeathEvent;
        }

        CurrentTarget = target;

        if (CurrentTarget != null)
        {
            CurrentTarget.DestroyEvent += OnDestroyEvent;
            CurrentTarget.DeathEvent += OnDeathEvent;
        }

        if (TargetSetEvent != null)
        {
            TargetSetEvent(CurrentTarget, previousTarget);
        }
    }

    private void OnDeathEvent(EnemyModel target)
    {
        FocusOnTarget(null);
    }

    private void OnDestroyEvent(BaseModel target)
    {
        FocusOnTarget(null);
    }
}
