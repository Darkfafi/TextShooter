using UnityEngine;

public class TurretModel : EntityModel
{
	public delegate void NewOldTargetHandler(EntityModel newTarget, EntityModel previousTarget);
	public event NewOldTargetHandler TargetSetEvent;

	public EntityModel CurrentTarget
	{
		get
		{
			if(TargetSystem == null)
				return null;

			return TargetSystem.TargetsFilter.GetFirst(
				(e) => 
				{
					if(!TargetSystem.IsTargetCompleted(e))
						return false;

					return true;
				}, (a, b) =>
				{
					float distA = (a.ModelTransform.Position - ModelTransform.Position).magnitude;
					float distB = (b.ModelTransform.Position - ModelTransform.Position).magnitude;
					return (int)(distA - distB);
				});
		}
	}

	public TargetSystem TargetSystem
	{
		get; private set;
	}

	public float TurretNeckRotation
	{
		get; private set;
	}

	public float Range
	{
		get; private set;
	}

	private TimekeeperModel _timekeeper;

	public TurretModel(TimekeeperModel timekeeper, CharInputModel charInputModel)
	{
		_timekeeper = timekeeper;
		_timekeeper.ListenToFrameTick(Update);

		TargetSystem = AddComponent<TargetSystem>();
		TargetSystem.SetupTargetSystem(charInputModel, FilterRules.CreateHasAnyTagsFilter(Tags.ENEMY));

		ModelTags.AddTag(Tags.DISPLAY_TARGETING);

		Range = 5f;
	}

	protected override void OnModelDestroy()
	{
		base.OnModelDestroy();

		_timekeeper.UnlistenFromFrameTick(Update);
		_timekeeper = null;

		
		TargetSystem = null;
	}

	private void Update(float deltaTime, float timeScale)
	{
		float angleToTarget = 0;

		if(CurrentTarget != null && !CurrentTarget.IsDestroyed)
		{
			if(Vector2.Distance(ModelTransform.Position, CurrentTarget.ModelTransform.Position) < Range)
			{
				float x = CurrentTarget.ModelTransform.Position.x - ModelTransform.Position.x;
				float y = CurrentTarget.ModelTransform.Position.y - ModelTransform.Position.y;

				angleToTarget = (Mathf.Atan2(y, x) * Mathf.Rad2Deg) - 90f;

				if(Mathf.Abs(Mathf.DeltaAngle(angleToTarget, TurretNeckRotation)) < 10f)
				{
					// TODO: Fire bullet which does the hit logic for the turret in its own fassion. 
					if(CurrentTarget.GetComponent<WordsHp>().HitEntireWord())
					{
						Debug.Log("HIT");
					}
				}
			}
		}

		TurretNeckRotation = Mathf.LerpAngle(TurretNeckRotation, angleToTarget, deltaTime * timeScale * 7.4f);
	}
}
