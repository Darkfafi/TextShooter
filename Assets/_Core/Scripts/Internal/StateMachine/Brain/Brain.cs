using System;
using System.Collections.Generic;

public class Brain<T> : IBrain<T> where T : class
{
	public bool IsEnabled
	{
		get; private set;
	}

	public StateMachine<T> BrainStateMachine
	{
		get; private set;
	}

	public IBrainState BrainState
	{
		get
		{
			return _brainState;
		}
	}

	private BrainState<T> _brainState;

	private List<BaseBrainSwitcher<T>> _globalSwitchers = new List<BaseBrainSwitcher<T>>(); // Always active
	private List<BaseBrainSwitcher<T>> _noStateSwitchers = new List<BaseBrainSwitcher<T>>(); // Only when no state active
	private Dictionary<Type, List<BaseBrainSwitcher<T>>> _stateSwitchers = new Dictionary<Type, List<BaseBrainSwitcher<T>>>(); // Only active for specific state
	private Type _currentStateSwitchersType = null;

	public Brain(T affected, bool isEnabledFromStart = true)
	{
		_brainState = new BrainState<T>(this);
		BrainStateMachine = new StateMachine<T>(affected);
		BrainStateMachine.StateSetEvent += OnStateSetEvent;

		if(isEnabledFromStart)
		{
			SetEnabledState(true);
		}
	}

	public void SetEnabledState(bool enabledState)
	{
		if(IsEnabled == enabledState)
			return;

		IsEnabled = enabledState;

		if(IsEnabled)
		{
			StateConditionSetGlobalSwitchers();
			StateConditionSetStateSwitchers(BrainStateMachine.CurrentStateType);
		}
		else
		{
			DeactivateGlobalSwitchers();
			DeactivateStateSwitchers(BrainStateMachine.CurrentStateType);
			BrainStateMachine.RequestNoState(false);
		}
	}

	public void SetupNoStateSwitcher<U>(U switcher) where U : BaseBrainSwitcher<T>
	{
		SetupStateSwitcher(switcher, null);
	}

	public void SetupStateSwitcher<U, V>(U switcher) where U : BaseBrainSwitcher<T> where V : class, IStateMachineState<T>
	{
		SetupStateSwitcher(switcher, typeof(V));
	}

	public void SetupStateSwitcher<U>(U switcher, Type stateType) where U : BaseBrainSwitcher<T>
	{
		if(stateType != null)
		{
			if(!_stateSwitchers.ContainsKey(stateType))
			{
				_stateSwitchers.Add(stateType, new List<BaseBrainSwitcher<T>>());
			}

			_stateSwitchers[stateType].Add(switcher);
			switcher.Initialize(this);

			if(BrainStateMachine.CurrentStateType == stateType)
			{
				switcher.Activate(true);
			}
		}
		else
		{
			_noStateSwitchers.Add(switcher);
			switcher.Initialize(this);

			if(BrainStateMachine.CurrentStateType == null)
			{
				switcher.Activate(true);
			}
		}
	}

	public void SetupGlobalSwitcher<U>(U switcher) where U : BaseBrainSwitcher<T>
	{
		_globalSwitchers.Add(switcher);
		switcher.Initialize(this);

		if(IsEnabled)
		{
			switcher.Activate(true);
		}
	}

	public void Clean()
	{
		BrainStateMachine.StateSetEvent -= OnStateSetEvent;

		DeactivateGlobalSwitchers();
		DeactivateStateSwitchers(BrainStateMachine.CurrentStateType);

		BrainStateMachine.Clean();
		BrainStateMachine = null;

		_globalSwitchers.Clear();
		_globalSwitchers = null;

		_noStateSwitchers.Clear();
		_noStateSwitchers = null;

		_stateSwitchers.Clear();
		_stateSwitchers = null;
	}

	private void OnStateSetEvent(IStateMachineState<T> state)
	{
		DeactivateStateSwitchers(_currentStateSwitchersType);
		_currentStateSwitchersType = state == null ? null : state.GetType();
		StateConditionSetGlobalSwitchers();
		StateConditionSetStateSwitchers(_currentStateSwitchersType);
	}

	private void StateConditionSetGlobalSwitchers()
	{
		for(int i = 0, c = _globalSwitchers.Count; i < c; i++)
		{
			if(_globalSwitchers[i].ConditionMet())
			{
				_globalSwitchers[i].Activate(false);
			}
			else
			{
				_globalSwitchers[i].Deactivate();
			}
		}
	}

	private void DeactivateGlobalSwitchers()
	{
		for(int i = 0, c = _globalSwitchers.Count; i < c; i++)
		{
			_globalSwitchers[i].Deactivate();
		}
	}

	private void StateConditionSetStateSwitchers(Type stateType)
	{
		List<BaseBrainSwitcher<T>> switchers;

		if(stateType != null)
		{
			_stateSwitchers.TryGetValue(stateType, out switchers);
		}
		else
		{
			switchers = _noStateSwitchers;
		}

		if(switchers != null)
		{
			for(int i = 0, c = switchers.Count; i < c; i++)
			{
				if(switchers[i].ConditionMet())
				{
					switchers[i].Activate(false);
				}
				else
				{
					switchers[i].Deactivate();
				}
			}
		}
	}

	private void DeactivateStateSwitchers(Type stateType)
	{
		List<BaseBrainSwitcher<T>> switchers;

		if(stateType != null)
		{
			_stateSwitchers.TryGetValue(stateType, out switchers);
		}
		else
		{
			switchers = _noStateSwitchers;
		}

		if(switchers != null)
		{
			for(int i = 0, c = switchers.Count; i < c; i++)
			{
				switchers[i].Deactivate();
			}
		}
	}
}

public interface IBrain<T> where T : class
{
	bool IsEnabled
	{
		get;
	}

	StateMachine<T> BrainStateMachine
	{
		get;
	}

	void SetEnabledState(bool enabledState);
	void SetupNoStateSwitcher<U>(U switcher) where U : BaseBrainSwitcher<T>;
	void SetupStateSwitcher<U, V>(U switcher) where U : BaseBrainSwitcher<T> where V : class, IStateMachineState<T>;
	void SetupStateSwitcher<U>(U switcher, Type stateType) where U : BaseBrainSwitcher<T>;
	void SetupGlobalSwitcher<U>(U switcher) where U : BaseBrainSwitcher<T>;
}

