using System;

public class EntityBrain : ModelBrain<EntityModel>
{

}

public class ModelBrain : ModelBrain<BaseModel>
{

}

public class ModelBrain<T> : BaseModelComponent, IBrain<T> where T : BaseModel
{
	public StateMachine<T> BrainStateMachine
	{
		get
		{
			return _internalBrain.BrainStateMachine;
		}
	}

	private Brain<T> _internalBrain;

	protected override void Added()
	{
		_internalBrain = new Brain<T>((T)Parent, true);
	}

	protected override void Removed()
	{
		_internalBrain.Clean();
		_internalBrain = null;
	}

	protected override void Enabled()
	{
		_internalBrain.SetEnabledState(true);
	}

	protected override void Disabled()
	{
		_internalBrain.SetEnabledState(false);
	}

	public void SetupNoStateSwitcher<U>(U switcher) where U : BaseBrainSwitcher<T>
	{
		_internalBrain.SetupNoStateSwitcher(switcher);
	}

	public void SetupStateSwitcher<U>(U switcher, Type stateType) where U : BaseBrainSwitcher<T>
	{
		_internalBrain.SetupStateSwitcher(switcher, stateType);
	}

	public void SetupGlobalSwitcher<U>(U switcher) where U : BaseBrainSwitcher<T>
	{
		_internalBrain.SetupGlobalSwitcher(switcher);
	}

	public void SetupStateSwitcher<U, V>(U switcher) where U : BaseBrainSwitcher<T> where V : class, IStateMachineState<T>
	{
		_internalBrain.SetupStateSwitcher<U, V>(switcher);
	}
}