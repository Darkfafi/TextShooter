public class MoveIntoRangeSwitcher : BaseBrainSwitcher<EntityModel>
{
	private EntityFilter<EntityModel> _targetsFilter;
	private MoveInRangeSwitcherData _data;

	public MoveIntoRangeSwitcher(MoveInRangeSwitcherData data)
	{
		_data = data;

		FilterRules r = _data.TargetFilterRules; 
		FilterRules.OpenConstructOnFilterRules(r);
		FilterRules.AddComponentToConstruct<Lives>(true);
		FilterRules.CloseConstruct(out r);

		_targetsFilter = EntityFilter<EntityModel>.Create(r);
	}

	protected override PotentialSwitch<EntityModel>? CheckForSwitchRequest()
	{
		int prio;
		EntityModel target = GetTargetCalculatePrio(out prio);
		if(target != null && prio != SwitcherSettings.NO_PRIO)
		{
			MovementStateRequest r = new MovementStateRequest(target, _data.RangeToMoveTo);

			if(_data.SpecifiedSpeed.HasValue)
				r.SpecifySpeed(_data.SpecifiedSpeed.Value);
			
			return CreatePotentialSwitchToState(r, prio);
		}

		return null;
	}

	protected override void Destroyed()
	{
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