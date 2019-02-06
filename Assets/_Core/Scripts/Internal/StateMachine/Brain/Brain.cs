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

	private List<IBrainSwitcher<T>> _globalSwitchers = new List<IBrainSwitcher<T>>(); // Always active
	private List<IBrainSwitcher<T>> _noStateSwitchers = new List<IBrainSwitcher<T>>(); // Only when no state active
	private Dictionary<Type, List<IBrainSwitcher<T>>> _stateSwitchers = new Dictionary<Type, List<IBrainSwitcher<T>>>(); // Only active for specific state
	private List<IBrainSwitcher<T>> _activeSwitchers = new List<IBrainSwitcher<T>>();

	private Type _currentStateSwitchersType = null;
	private TimekeeperModel _timekeeperModel;
	private bool _addedOwnStateMachine = false;

	public Brain(TimekeeperModel timekeeperModel, T affected, bool isEnabledFromStart = true)
	{
		_addedOwnStateMachine = true;
		Initialize(timekeeperModel, new StateMachine<T>(affected), isEnabledFromStart);
	}

	public Brain(TimekeeperModel timekeeperModel, StateMachine<T> stateMachine, bool isEnabledFromStart = true)
	{
		Initialize(timekeeperModel, stateMachine, isEnabledFromStart);
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
			_timekeeperModel.ListenToFrameTick(OnUpdate);
		}
		else
		{
			_timekeeperModel.UnlistenFromFrameTick(OnUpdate);
			DeactivateGlobalSwitchers();
			DeactivateStateSwitchers(BrainStateMachine.CurrentStateType);
			BrainStateMachine.RequestNoState(false);
		}
	}

	public void SetupNoStateSwitcher(IBrainSwitcher<T> switcher)
	{
		SetupStateSwitcher(switcher, null);
	}

	public void SetupStateSwitcher<V>(IBrainSwitcher<T> switcher) where V : class, IStateMachineState<T>
	{
		SetupStateSwitcher(switcher, typeof(V));
	}

	public void SetupStateSwitcher(IBrainSwitcher<T> switcher, Type stateType)
	{
		if(stateType != null)
		{
			if(!_stateSwitchers.ContainsKey(stateType))
			{
				_stateSwitchers.Add(stateType, new List<IBrainSwitcher<T>>());
			}

			_stateSwitchers[stateType].Add(switcher);
			switcher.Initialize(this);

			if(BrainStateMachine.CurrentStateType == stateType)
			{
				SetSwitcherActiveState(switcher, true);
			}
		}
		else
		{
			_noStateSwitchers.Add(switcher);
			switcher.Initialize(this);

			if(BrainStateMachine.CurrentStateType == null)
			{
				SetSwitcherActiveState(switcher, true);
			}
		}
	}

	public void SetupGlobalSwitcher(IBrainSwitcher<T> switcher)
	{
		_globalSwitchers.Add(switcher);
		switcher.Initialize(this);

		if(IsEnabled)
		{
			SetSwitcherActiveState(switcher, true);
		}
	}

	public void Clean()
	{
		_timekeeperModel.UnlistenFromFrameTick(OnUpdate);
		_timekeeperModel = null;

		BrainStateMachine.StateSetEvent -= OnStateSetEvent;

		DeactivateGlobalSwitchers();
		DeactivateStateSwitchers(BrainStateMachine.CurrentStateType);

		if(_addedOwnStateMachine)
			BrainStateMachine.Clean();

		BrainStateMachine = null;
		
		_globalSwitchers.Clear();
		_globalSwitchers = null;

		_noStateSwitchers.Clear();
		_noStateSwitchers = null;

		_stateSwitchers.Clear();
		_stateSwitchers = null;
	}

	private void OnUpdate(float deltaTime, float timeScale)
	{
		if(BrainStateMachine == null || !IsEnabled)
			return;

		List<IBrainSwitcher<T>> switchers = new List<IBrainSwitcher<T>>(_activeSwitchers);
		List<PotentialSwitch<T>> potentialSwitches = new List<PotentialSwitch<T>>();

		int rollMaxValue = 0;
		for(int i = 0, c = switchers.Count; i < c; i++)
		{
			PotentialSwitch<T>? potentialSwitch = switchers[i].CallCheckForSwitchRequest();
			if(potentialSwitch.HasValue && potentialSwitch.Value.IsSet && potentialSwitch.Value.PriorityLevel > 0)
			{
				if(potentialSwitch.Value.Request != null && !potentialSwitch.Value.Request.IsAllowedToCreate())
				{
					potentialSwitch.Value.Clean();
					continue;
				}

				potentialSwitches.Add(potentialSwitch.Value);
				rollMaxValue += potentialSwitch.Value.PriorityLevel;
			}
			else if(potentialSwitch.HasValue)
			{
				potentialSwitch.Value.Clean();
			}
		}

		potentialSwitches.Sort((a, b) =>
		{
			return b.PriorityLevel - a.PriorityLevel;
		});

		int choiceRoll = UnityEngine.Random.Range(0, rollMaxValue  + 1);
		int currentPrioLevel = 0;
		PotentialSwitch<T> chosenPotentialSwitch = new PotentialSwitch<T>();

		for(int i = 0, c = potentialSwitches.Count; i < c; i++)
		{
			PotentialSwitch<T> pr = potentialSwitches[i];
			if(!chosenPotentialSwitch.IsSet)
			{
				if(choiceRoll > currentPrioLevel && choiceRoll <= currentPrioLevel + pr.PriorityLevel)
				{
					chosenPotentialSwitch = pr;
				}
				else
				{
					pr.Clean();
				}

				currentPrioLevel += pr.PriorityLevel;
			}
			else
			{
				pr.Clean();
			}
		}

		if(chosenPotentialSwitch.IsSet)
		{
			if(chosenPotentialSwitch.Request == null)
			{
				BrainStateMachine.RequestNoState(chosenPotentialSwitch.Force);
			}
			else
			{
				BrainStateMachine.RequestState(chosenPotentialSwitch.Request, chosenPotentialSwitch.Force);
			}

			foreach(var pair in chosenPotentialSwitch.Switcher.GetKeysToSetOnRequestDictionary())
			{
				BrainState.SetKey(pair.Key, pair.Value);
			}
		}
	}

	private void Initialize(TimekeeperModel timekeeperModel, StateMachine<T> stateMachine, bool isEnabledFromStart)
	{
		if(_timekeeperModel != null)
			return;

		_timekeeperModel = timekeeperModel;
		_brainState = new BrainState<T>(this);
		BrainStateMachine = stateMachine;
		BrainStateMachine.StateSetEvent += OnStateSetEvent;

		if(isEnabledFromStart)
		{
			SetEnabledState(true);
		}
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
				SetSwitcherActiveState(_globalSwitchers[i], true);
			}
			else
			{
				SetSwitcherActiveState(_globalSwitchers[i], false);
			}
		}
	}

	private void DeactivateGlobalSwitchers()
	{
		for(int i = 0, c = _globalSwitchers.Count; i < c; i++)
		{
			SetSwitcherActiveState(_globalSwitchers[i], false);
		}
	}

	private void StateConditionSetStateSwitchers(Type stateType)
	{
		List<IBrainSwitcher<T>> switchers = GetStateSwitchers(stateType);

		if(switchers != null)
		{
			for(int i = 0, c = switchers.Count; i < c; i++)
			{
				if(switchers[i].ConditionMet())
				{
					SetSwitcherActiveState(switchers[i], true);
				}
				else
				{
					SetSwitcherActiveState(switchers[i], false);
				}
			}
		}
	}

	private void DeactivateStateSwitchers(Type stateType)
	{
		List<IBrainSwitcher<T>> switchers = GetStateSwitchers(stateType);

		if(switchers != null)
		{
			for(int i = 0, c = switchers.Count; i < c; i++)
			{
				SetSwitcherActiveState(switchers[i], false);
			}
		}
	}

	private void SetSwitcherActiveState(IBrainSwitcher<T> switcher, bool activeState)
	{
		if(activeState)
		{
			if(switcher.Activate(true) && !_activeSwitchers.Contains(switcher))
			{
				_activeSwitchers.Add(switcher);
			}
		}
		else
		{
			if(switcher.Deactivate())
			{
				_activeSwitchers.Remove(switcher);
			}
		}
	}

	private List<IBrainSwitcher<T>> GetStateSwitchers(Type stateType)
	{
		List<IBrainSwitcher<T>> switchers;

		if(stateType != null)
		{
			if(!_stateSwitchers.TryGetValue(stateType, out switchers))
			{
				return new List<IBrainSwitcher<T>>();
			}
		}
		else
		{
			switchers = _noStateSwitchers;
		}

		return switchers;
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
	void SetupNoStateSwitcher(IBrainSwitcher<T> switcher);
	void SetupStateSwitcher<V>(IBrainSwitcher<T> switcher) where V : class, IStateMachineState<T>;
	void SetupStateSwitcher(IBrainSwitcher<T> switcher, Type stateType);
	void SetupGlobalSwitcher(IBrainSwitcher<T> switcher);
}

