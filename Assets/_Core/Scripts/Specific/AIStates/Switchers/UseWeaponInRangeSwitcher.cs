public class UseWeaponInRangeSwitcher : BaseBrainSwitcher<EntityModel>
{
	private TimekeeperModel _timekeeperModel;
	private EntityFilter<EntityModel> _targetsFilter;
	private float _useAtRadiusPercentage;

	public UseWeaponInRangeSwitcher(TimekeeperModel timekeeperModel, FilterRules targetFilterRules, float useAtRadiusPercentage = 1f)
	{
		useAtRadiusPercentage = UnityEngine.Mathf.Clamp01(useAtRadiusPercentage);

		_timekeeperModel = timekeeperModel;
		_useAtRadiusPercentage = useAtRadiusPercentage;
		FilterRules.OpenConstructOnFilterRules(targetFilterRules);
		FilterRules.AddComponentToConstruct<Lives>(true);
		FilterRules.CloseConstruct(out targetFilterRules);

		_targetsFilter = EntityFilter<EntityModel>.Create(targetFilterRules); 
	}

	protected override void Initialized()
	{

	}

	protected override void Activated()
	{
		_timekeeperModel.ListenToFrameTick(OnUpdate);
	}

	protected override void Deactivated()
	{
		_timekeeperModel.UnlistenFromFrameTick(OnUpdate);
	}

	protected override void Destroyed()
	{
		_targetsFilter.Clean();
		_targetsFilter = null;

		_timekeeperModel = null;
	}

	private void OnUpdate(float deltaTime, float timeScale)
	{
		if(Affected == null)
			return;

		EntityModel closestTarget = _targetsFilter.GetFirst(e => e.GetComponent<Lives>().IsAlive, Affected.SortOnClosestTo());
		if(closestTarget != null)
		{
			float distance = (closestTarget.ModelTransform.Position - Affected.ModelTransform.Position).magnitude;
			if(Affected.HasComponent<BaseWeapon>())
			{
				BaseWeapon weapon = Affected.GetComponent<BaseWeapon>();
				if(!weapon.CanBeUsed || distance > (weapon.Radius * _useAtRadiusPercentage))
					return;
			}
			else
			{
				return;
			}

			RequestState(new UseWeaponStateRequest(closestTarget.GetComponent<Lives>()));
		}
	}
}
