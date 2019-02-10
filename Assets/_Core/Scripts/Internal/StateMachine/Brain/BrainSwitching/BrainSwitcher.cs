using System;
using System.Collections.Generic;

public enum BrainSwitcherStatus
{
	NotReady,
	Initialized,
	Activated,
	Deactivated,
	Destroyed
}

public class BrainSwitcher<T, U> : IBrainSwitcher<T> where T : class where U : BaseSwitcherDataObject<T>
{
	public delegate PotentialSwitch<T>? PotentialSwitchCallHandler(T affected, U dataObject);

	public BrainSwitcherStatus SwitcherStatus
	{
		get; private set;
	}

	public T Affected
	{
		get
		{
			if(_brain == null || _brain.BrainStateMachine == null)
				return null;
			return _brain.BrainStateMachine.Affected;
		}
	}

	public Type CurrentStateType
	{
		get
		{
			if(_brain == null || _brain.BrainStateMachine == null)
				return null;

			return _brain.BrainStateMachine.CurrentStateType;
		}
	}

	public bool IsCurrentStateValid
	{
		get
		{
			if(_brain == null || _brain.BrainStateMachine == null)
				return false;

			return _brain.BrainStateMachine.IsCurrentStateValid;
		}
	}

	private Dictionary<string, bool> _conditionKeys = new Dictionary<string, bool>();
	private PotentialSwitchCallHandler _potentialSwitchGetter;
	private U _dataObject;
	private Brain<T> _brain;

	public BrainSwitcher(PotentialSwitchCallHandler potentialSwitchGetter, U dataObject)
	{
		_dataObject = dataObject;
		_potentialSwitchGetter = potentialSwitchGetter;
		SwitcherStatus = BrainSwitcherStatus.NotReady;
	}

	public PotentialSwitch<T>? CallCheckForSwitchRequest()
	{
		return _potentialSwitchGetter(Affected, _dataObject);
	}

	public void Initialize(Brain<T> brain)
	{
		if(SwitcherStatus != BrainSwitcherStatus.NotReady)
			return;
		_brain = brain;
		SwitcherStatus = BrainSwitcherStatus.Initialized;
		_dataObject.Initialized();
	}

	public void Clean()
	{
		if(SwitcherStatus == BrainSwitcherStatus.Destroyed)
			return;

		Deactivate();
		SwitcherStatus = BrainSwitcherStatus.Destroyed;
		_dataObject.Destroyed();
		_potentialSwitchGetter = null;
		_brain = null;
	}

	public bool Activate(bool onlyIfConditionMet)
	{
		if(SwitcherStatus != BrainSwitcherStatus.Initialized && SwitcherStatus != BrainSwitcherStatus.Deactivated)
			return false;

		if(onlyIfConditionMet && !ConditionMet())
			return false;

		SwitcherStatus = BrainSwitcherStatus.Activated;
		_dataObject.Activated();
		return true;
	}

	public bool Deactivate()
	{
		if(SwitcherStatus != BrainSwitcherStatus.Activated)
			return false;

		SwitcherStatus = BrainSwitcherStatus.Deactivated;
		_dataObject.Deactivated();
		return true;
	}

	public bool ConditionMet()
	{
		if(_brain == null || _brain.BrainState == null)
			return false;

		foreach(var pair in _conditionKeys)
		{
			if(_brain.BrainState.GetKey(pair.Key) != pair.Value)
				return false;
		}

		return true;
	}

	public IBrainSwitcher<T> SetConditionKey(string key, bool value = true)
	{
		if(!_conditionKeys.ContainsKey(key))
		{
			_conditionKeys.Add(key, value);
		}
		else
		{
			_conditionKeys[key] = value;
		}

		return this;
	}
}

public interface IBrainSwitcher<T> where T : class
{
	BrainSwitcherStatus SwitcherStatus
	{
		get;
	}

	T Affected
	{
		get;
	}

	PotentialSwitch<T>? CallCheckForSwitchRequest();
	bool Activate(bool onlyIfConditionMet);
	bool Deactivate();
	bool ConditionMet();
	void Initialize(Brain<T> brain);
	void Clean();

	IBrainSwitcher<T> SetConditionKey(string key, bool value = true);
}