public class MoveIntoRangeSwitcher : BaseBrainSwitcher<EntityModel>
{
	private EntityFilter<EntityModel> _targetsFilter;
	private MoveInRangeSwitcherData _data;
	private EntityModel _lastCalculatedTarget;

	public MoveIntoRangeSwitcher(MoveInRangeSwitcherData data)
	{
		_data = data;

		FilterRules r = _data.TargetFilterRules; 
		FilterRules.OpenConstructOnFilterRules(r);
		FilterRules.AddComponentToConstruct<Lives>(true);
		FilterRules.CloseConstruct(out r);

		_targetsFilter = EntityFilter<EntityModel>.Create(r);
	}

	protected override int CalculatePriorityLevel()
	{
		int prio;
		_lastCalculatedTarget = GetTargetCalculatePrio(out prio);
		return prio;
	}

	protected override void OnSwitchIfDesired()
	{
		if(_lastCalculatedTarget != null && PriorityLevel != SwitcherSettings.NO_PRIO)
		{
			MovementStateRequest r = new MovementStateRequest(_lastCalculatedTarget, _data.RangeToMoveTo);

			if(_data.SpecifiedSpeed.HasValue)
				r.SpecifySpeed(_data.SpecifiedSpeed.Value);

			RequestState(r);
			_lastCalculatedTarget = null;
		}
	}

	protected override void Destroyed()
	{
		_lastCalculatedTarget = null;
		_targetsFilter.Clean();
		_targetsFilter = null;
	}

	private EntityModel GetTargetCalculatePrio(out int prio)
	{
		if(Affected == null)
		{
			prio = SwitcherSettings.NO_PRIO;
			return null;
		}

		EntityModel closestTarget = _targetsFilter.GetFirst(e => e.GetComponent<Lives>().IsAlive, Affected.SortOnClosestTo());

		if(closestTarget != null)
		{
			float distance = (closestTarget.ModelTransform.Position - Affected.ModelTransform.Position).magnitude;

			prio = SwitcherSettings.NORMAL_BASE_FULL_PRIO;

			if(_data.DistanceToTriggerSwitcher.HasValue)
			{
				if(_data.DistanceToTriggerSwitcher.Value > 0 && distance > _data.RangeToMoveTo)
				{
					prio = SwitcherSettings.NORMAL_BASE_FULL_PRIO + (int)(SwitcherSettings.NORMAL_BASE_FULL_PRIO * (1 - (distance / _data.DistanceToTriggerSwitcher.Value)));
				}
				else
				{
					prio = SwitcherSettings.NO_PRIO;
				}
			}

			return closestTarget;
		}
		else
		{
			prio = SwitcherSettings.NO_PRIO;
		}

		return null;
	}
}


public struct MoveInRangeSwitcherData
{
	public float RangeToMoveTo;
	public float? SpecifiedSpeed;
	public FilterRules TargetFilterRules;
	public float? DistanceToTriggerSwitcher;
}