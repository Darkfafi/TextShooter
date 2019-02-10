public class UseWeaponState : StateMachineState<EntityModel>
{
	private Lives _livesComponent;

	public void Setup(Lives livesComponent)
	{
		_livesComponent = livesComponent;
	}

	protected override void OnActivated()
	{
		if(Affected.HasComponent<WeaponHolder>())
		{
			Affected.GetComponent<WeaponHolder>().UseWeaponIfAny(_livesComponent);
		}

		EndStateInternally();
	}

	protected override void OnDeactivated()
	{
		_livesComponent = null;
	}
}

public class UseWeaponStateRequest : BaseStateMachineStateRequest<UseWeaponState, EntityModel>
{
	private Lives _targetComponent;

	public UseWeaponStateRequest(Lives targetLivesComponent)
	{
		_targetComponent = targetLivesComponent;
	}

	public override bool IsAllowedToCreate()
	{
		return _targetComponent != null && _targetComponent.ComponentState != ModelComponentState.Removed;
	}

	protected override void SetupCreatedState(UseWeaponState state)
	{
		state.Setup(_targetComponent);
	}

	public override void Clean()
	{
		_targetComponent = null;
	}
}