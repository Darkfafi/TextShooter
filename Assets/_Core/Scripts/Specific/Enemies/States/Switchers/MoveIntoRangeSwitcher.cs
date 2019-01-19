public class MoveIntoRangeSwitcher : BaseBrainSwitcher<EntityModel>
{
	private float _range;
	private TimekeeperModel _timekeeperModel;
	private EntityFilter<EntityModel> _targetsFilter;

	public MoveIntoRangeSwitcher(TimekeeperModel timekeeperModel, float range, FilterRules targetFilterRules)
	{
		_range = range;
		_timekeeperModel = timekeeperModel;
		_targetsFilter = EntityFilter<EntityModel>.Create(targetFilterRules);
	}

	protected override void Initialized()
	{

	}

	protected override void Activated()
	{
		_timekeeperModel.ListenToFrameTick(OnUpdate);
	}

	private void OnUpdate(float deltaTime, float timeScale)
	{
		if(timeScale > 0)
			TryActivateMovementState();
	}

	protected override void Deactivated()
	{
		_timekeeperModel.UnlistenFromFrameTick(OnUpdate);
	}

	protected override void Destroyed()
	{
		_timekeeperModel = null;

		_targetsFilter.Clean();
		_targetsFilter = null;
	}

	private void TryActivateMovementState()
	{
		if(Affected == null)
			return;

		EntityModel closestTarget = _targetsFilter.GetFirst((a, b) =>
		{
			float distA = (a.ModelTransform.Position - Affected.ModelTransform.Position).magnitude;
			float distB = (b.ModelTransform.Position - Affected.ModelTransform.Position).magnitude;
			return (int)(distA - distB);
		});

		if(closestTarget != null && (closestTarget.ModelTransform.Position - Affected.ModelTransform.Position).magnitude > _range)
		{
			RequestState(new MovementStateRequest(closestTarget, _range));
		}
	}
}
