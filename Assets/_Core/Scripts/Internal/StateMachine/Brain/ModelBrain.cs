using System;
using UnityEngine;

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

	public ModelBrain<T> Setup(TimekeeperModel timekeeper)
	{
		_internalBrain = new Brain<T>(timekeeper, (T)Parent, IsEnabled);
		return this;
	}

	public ModelBrain<T> Setup(TimekeeperModel timekeeper, StateMachine<T> stateMachine)
	{
		_internalBrain = new Brain<T>(timekeeper, stateMachine, IsEnabled);
		return this;
	}

	protected override void Added()
	{

	}

	protected override void Removed()
	{
		if(_internalBrain == null)
			return;

		_internalBrain.Clean();
		_internalBrain = null;
	}

	protected override void Enabled()
	{
		if(_internalBrain == null)
			return;

		_internalBrain.SetEnabledState(true);
	}

	protected override void Disabled()
	{
		if(_internalBrain == null)
			return;

		_internalBrain.SetEnabledState(false);
	}

	public void SetupNoStateSwitcher(IBrainSwitcher<T> switcher)
	{
		if(_internalBrain == null)
		{
			Debug.LogError("Trying to setup switcher without first calling 'Setup' on component");
			return;
		}

		_internalBrain.SetupNoStateSwitcher(switcher);
	}

	public void SetupStateSwitcher(IBrainSwitcher<T> switcher, Type stateType)
	{
		if(_internalBrain == null)
		{
			Debug.LogError("Trying to setup switcher without first calling 'Setup' on component");
			return;
		}

		_internalBrain.SetupStateSwitcher(switcher, stateType);
	}

	public void SetupGlobalSwitcher(IBrainSwitcher<T> switcher)
	{
		if(_internalBrain == null)
		{
			Debug.LogError("Trying to setup switcher without first calling 'Setup' on component");
			return;
		}

		_internalBrain.SetupGlobalSwitcher(switcher);
	}

	public void SetupStateSwitcher<V>(IBrainSwitcher<T> switcher) where V : class, IStateMachineState<T>
	{
		if(_internalBrain == null)
		{
			Debug.LogError("Trying to setup switcher without first calling 'Setup' on component");
			return;
		}

		_internalBrain.SetupStateSwitcher<V>(switcher);
	}
}