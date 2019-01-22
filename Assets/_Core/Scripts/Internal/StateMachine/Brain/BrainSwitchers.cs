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

public abstract class BaseBrainSwitcher<T> : IBrainSwitcher<T> where T : class
{
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

	private Dictionary<string, bool> _keysToSetOnStateRequest = new Dictionary<string, bool>();
	private Dictionary<string, bool> _conditionKeys = new Dictionary<string, bool>();
	private Brain<T> _brain;

	public BaseBrainSwitcher()
	{
		SwitcherStatus = BrainSwitcherStatus.NotReady;
	}

	public PotentialSwitch<T>? CallCheckForSwitchRequest()
	{
		return CheckForSwitchRequest();
	}

	public void Initialize(Brain<T> brain)
	{
		if(SwitcherStatus != BrainSwitcherStatus.NotReady)
			return;
		_brain = brain;
		SwitcherStatus = BrainSwitcherStatus.Initialized;
		Initialized();
	}

	public void Clean()
	{
		if(SwitcherStatus == BrainSwitcherStatus.Destroyed)
			return;

		Deactivate();
		SwitcherStatus = BrainSwitcherStatus.Destroyed;
		Destroyed();
		_brain = null;
	}

	public bool Activate(bool onlyIfConditionMet)
	{
		if(SwitcherStatus != BrainSwitcherStatus.Initialized && SwitcherStatus != BrainSwitcherStatus.Deactivated)
			return false;

		if(onlyIfConditionMet && !ConditionMet())
			return false;

		SwitcherStatus = BrainSwitcherStatus.Activated;
		Activated();
		return true;
	}

	public bool Deactivate()
	{
		if(SwitcherStatus != BrainSwitcherStatus.Activated)
			return false;

		SwitcherStatus = BrainSwitcherStatus.Deactivated;
		Deactivated();
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

	public Dictionary<string, bool> GetKeysToSetOnRequestDictionary()
	{
		return new Dictionary<string, bool>(_keysToSetOnStateRequest);
	}

	public BaseBrainSwitcher<T> SetConditionKey(string key, bool value = true)
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

	public BaseBrainSwitcher<T> SetOnRequestKeyToSet(string key, bool value = true)
	{
		if(!_keysToSetOnStateRequest.ContainsKey(key))
		{
			_keysToSetOnStateRequest.Add(key, value);
		}
		else
		{
			_keysToSetOnStateRequest[key] = value;
		}

		return this;
	}

	protected virtual void Initialized()
	{

	}

	protected virtual void Destroyed()
	{

	}

	protected virtual void Activated()
	{

	}

	protected virtual void Deactivated()
	{

	}

	protected PotentialSwitch<T> CreatePotentialSwitchToState(IStateMachineStateRequest<T> request, int priorityLevel, bool force = false)
	{
		return new PotentialSwitch<T>(this, request, priorityLevel, force);
	}

	protected PotentialSwitch<T> CreatePotentialSwitchToNoState(int priorityLevel, bool force)
	{
		return new PotentialSwitch<T>(this, null, priorityLevel, force);
	}

	protected abstract PotentialSwitch<T>? CheckForSwitchRequest();
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

	bool ConditionMet();
	void Initialize(Brain<T> brain);
	void Clean();
}

public struct PotentialSwitch<T> where T : class
{
	public IStateMachineStateRequest<T> Request
	{
		get; private set;
	}

	public BaseBrainSwitcher<T> Switcher
	{
		get; private set;
	}

	public int PriorityLevel
	{
		get; private set;
	}

	public bool Force
	{
		get; private set;
	}

	public bool IsSet
	{
		get; private set;
	}

	public PotentialSwitch(BaseBrainSwitcher<T> switcher, IStateMachineStateRequest<T> request, int priorityLevel, bool force = false)
	{
		Switcher = switcher;
		Request = request;
		PriorityLevel = priorityLevel;
		IsSet = true;
		Force = force;
	}

	public void Clean()
	{
		if(Request != null)
			Request.Clean();

		Switcher = null;
		Request = null;
	}
}
