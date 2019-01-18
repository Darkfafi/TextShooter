public class MoveIntoRangeSwitcher : BaseBrainSwitcher<EntityModel>
{
	private float _range;
	private EntityFilter<EntityModel> _targetsFilter;

	public MoveIntoRangeSwitcher(float range, FilterRules targetFilterRules)
	{
		_range = range;
		_targetsFilter = EntityFilter<EntityModel>.Create(targetFilterRules);
		_targetsFilter.TrackedEvent += OnTrackedEvent;
	}

	protected override void Initialized()
	{

	}

	protected override void Activated()
	{
		TryActivateMovementState();
	}

	protected override void Deactivated()
	{

	}

	protected override void Destroyed()
	{
		_targetsFilter.TrackedEvent -= OnTrackedEvent;
		_targetsFilter.Clean();
		_targetsFilter = null;
	}

	private void OnTrackedEvent(EntityModel entity)
	{
		TryActivateMovementState();
	}

	private void TryActivateMovementState()
	{
		EntityModel closestTarget = _targetsFilter.GetFirst((a, b) =>
		{
			float distA = (a.ModelTransform.Position - Affected.ModelTransform.Position).magnitude;
			float distB = (b.ModelTransform.Position - Affected.ModelTransform.Position).magnitude;
			return (int)(distA - distB);
		});

		if(closestTarget != null)
		{
			BrainStateMachine.RequestState(new MovementStateRequest(closestTarget, _range));
		}
	}
}
