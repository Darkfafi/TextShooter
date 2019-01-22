public class UseWeaponInRangeSwitcher : BaseBrainSwitcher<EntityModel>
{
	private EntityFilter<EntityModel> _targetsFilter;
	private float _useAtRadiusPercentage;

	public UseWeaponInRangeSwitcher(FilterRules targetFilterRules, float useAtRadiusPercentage = 1f)
	{
		useAtRadiusPercentage = UnityEngine.Mathf.Clamp01(useAtRadiusPercentage);

		_useAtRadiusPercentage = useAtRadiusPercentage;
		FilterRules.OpenConstructOnFilterRules(targetFilterRules);
		FilterRules.AddComponentToConstruct<Lives>(true);
		FilterRules.CloseConstruct(out targetFilterRules);

		_targetsFilter = EntityFilter<EntityModel>.Create(targetFilterRules); 
	}

	protected override void Destroyed()
	{
		_targetsFilter.Clean();
		_targetsFilter = null;
	}

	protected override PotentialSwitch<EntityModel>? CheckForSwitchRequest()
	{
		int prio;
		EntityModel target = GetTargetCalculatePrio(out prio);

		if(target != null && prio != SwitcherSettings.NO_PRIO)
		{
			return CreatePotentialSwitchToState(new UseWeaponStateRequest(target.GetComponent<Lives>()), prio);
		}

		return null;
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
