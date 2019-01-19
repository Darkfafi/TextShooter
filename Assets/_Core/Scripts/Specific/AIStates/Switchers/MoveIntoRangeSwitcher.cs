﻿public class MoveIntoRangeSwitcher : BaseBrainSwitcher<EntityModel>
{
	private TimekeeperModel _timekeeperModel;
	private EntityFilter<EntityModel> _targetsFilter;
	private MoveInRangeSwitcherData _data;

	public MoveIntoRangeSwitcher(TimekeeperModel timekeeperModel, MoveInRangeSwitcherData data)
	{
		_timekeeperModel = timekeeperModel;
		_data = data;
		_targetsFilter = EntityFilter<EntityModel>.Create(_data.TargetFilterRules);
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


		if(closestTarget != null)
		{
			float distance = (closestTarget.ModelTransform.Position - Affected.ModelTransform.Position).magnitude;

			if(_data.DistanceToTriggerSwitcher.HasValue)
			{
				if(_data.DistanceToTriggerSwitcher > distance)
					return;
			}

			if(distance > _data.RangeToMoveTo)
			{
				MovementStateRequest r = new MovementStateRequest(closestTarget, _data.RangeToMoveTo);

				if(_data.SpecifiedSpeed.HasValue)
					r.SpecifySpeed(_data.SpecifiedSpeed.Value);

				RequestState(r);
			}
		}
	}
}


public struct MoveInRangeSwitcherData
{
	public float RangeToMoveTo;
	public float? SpecifiedSpeed;
	public FilterRules TargetFilterRules;
	public float? DistanceToTriggerSwitcher;
}