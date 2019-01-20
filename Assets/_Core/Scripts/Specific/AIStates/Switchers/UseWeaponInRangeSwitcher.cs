public class UseWeaponInRangeSwitcher : BaseBrainSwitcher<EntityModel>
{
	private EntityFilter<EntityModel> _targetsFilter;
	private float _useAtRadiusPercentage;
	private EntityModel _lastCalculatedTarget;

	public UseWeaponInRangeSwitcher(FilterRules targetFilterRules, float useAtRadiusPercentage = 1f)
	{
		useAtRadiusPercentage = UnityEngine.Mathf.Clamp01(useAtRadiusPercentage);

		_useAtRadiusPercentage = useAtRadiusPercentage;
		FilterRules.OpenConstructOnFilterRules(targetFilterRules);
		FilterRules.AddComponentToConstruct<Lives>(true);
		FilterRules.CloseConstruct(out targetFilterRules);

		_targetsFilter = EntityFilter<EntityModel>.Create(targetFilterRules); 
	}

	protected override int CalculatePriorityLevel()
	{
		int prio;
		_lastCalculatedTarget = GetTargetCalculatePrio(out prio);
		return prio;
	}

	protected override void Destroyed()
	{
		_targetsFilter.Clean();
		_targetsFilter = null;
	}

	protected override void OnSwitchIfDesired()
	{
		if(_lastCalculatedTarget != null && PriorityLevel != SwitcherSettings.NO_PRIO)
		{
			RequestState(new UseWeaponStateRequest(_lastCalculatedTarget.GetComponent<Lives>()));
		}
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
			if(Affected.HasComponent<BaseWeapon>())
			{
				BaseWeapon weapon = Affected.GetComponent<BaseWeapon>();
				if(!weapon.CanBeUsed || distance > (weapon.Radius * _useAtRadiusPercentage))
				{
					prio = SwitcherSettings.NO_PRIO;
					return closestTarget;
				}
			}
			else
			{
				prio = SwitcherSettings.NO_PRIO;
				return closestTarget;
			}

			prio = SwitcherSettings.NORMAL_BASE_FULL_PRIO;

			return closestTarget;

		}

		prio = SwitcherSettings.NO_PRIO;
		return null;
	}
}
