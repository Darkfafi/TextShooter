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

public abstract class BaseBrainSwitcher<T> : IBrainSwitcher<T>, IStateMachine<T> where T : class
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

	public void Activate(bool onlyIfConditionMet)
	{
		if(SwitcherStatus != BrainSwitcherStatus.Initialized && SwitcherStatus != BrainSwitcherStatus.Deactivated)
			return;

		if(onlyIfConditionMet && !ConditionMet())
			return;

		SwitcherStatus = BrainSwitcherStatus.Activated;
		Activated();
	}

	public void Deactivate()
	{
		if(SwitcherStatus != BrainSwitcherStatus.Activated)
			return;

		SwitcherStatus = BrainSwitcherStatus.Deactivated;
		Deactivated();
	}

	public void RequestState(IStateMachineStateRequest<T> request, bool force = false)
	{
		if(_brain != null && _brain.BrainStateMachine != null)
		{
			SetRequestKeys();
			_brain.BrainStateMachine.RequestState(request, force);
		}
	}

	public void RequestNoState(bool force)
	{
		if(_brain != null && _brain.BrainStateMachine != null)
		{
			SetRequestKeys();
			_brain.BrainStateMachine.RequestNoState(force);
		}
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

	protected abstract void Initialized();
	protected abstract void Destroyed();
	protected abstract void Activated();
	protected abstract void Deactivated();

	private void SetRequestKeys()
	{
		if(_brain == null || _brain.BrainStateMachine == null)
		{
			foreach(var pair in _keysToSetOnStateRequest)
			{
				_brain.BrainState.SetKey(pair.Key, pair.Value);
			}
		}
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
