public class TargetsSwitcherDataObject : BaseSwitcherDataObject<EntityModel>
{
	public EntityFilter<EntityModel> TargetsFilter
	{
		get; private set;
	}

	public TargetsSwitcherDataObject(FilterRules filterRules)
	{
		FilterRules r = filterRules; 
		FilterRules.OpenConstructOnFilterRules(r);
		FilterRules.AddComponentToConstruct<Lives>(true);
		FilterRules.CloseConstruct(out r);

		TargetsFilter = EntityFilter<EntityModel>.Create(r);
	}

	protected override void Destroyed()
	{
		TargetsFilter.Clean();
		TargetsFilter = null;
	}
}