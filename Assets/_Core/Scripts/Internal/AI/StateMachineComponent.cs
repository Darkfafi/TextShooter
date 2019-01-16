using System;
using System.Collections.Generic;
using UnityEngine;

public class StateMachineComponent : BaseModelComponent
{
	public event Action<StateMachineState> StateSetEvent;
	private Dictionary<Type, StateMachineState> _states = new Dictionary<Type, StateMachineState>();
	private StateStatus _currentStateStatus = new StateStatus();

	public Type CurrentStateType
	{
		get
		{
			return _currentStateStatus.StateType;
		}
	}

	public bool IsCurrentStateValid
	{
		get
		{
			return _currentStateStatus.IsValidState;
		}
	}

	public void SetState<T>() where T : StateMachineState
	{
		Type t = typeof(T);
		if(!_states.ContainsKey(t))
		{
			Debug.LogWarning("No state found of type " + t.ToString());
			return;
		}

		if(_currentStateStatus.IsValidState)
		{
			_currentStateStatus.State.Deactivate();
		}

		StateStatus newState = new StateStatus();
		newState.StateType = t;
		newState.State = _states[t];

		_currentStateStatus = newState;

		_currentStateStatus.State.Activate();

		if(StateSetEvent != null)
		{
			StateSetEvent(_currentStateStatus.State);
		}

	}

	public void SetupState<T>(T state) where T : StateMachineState
	{
		Type t = typeof(T);
		if(t == null)
		{
			Debug.LogError("A state can't be null");
			return;
		}

		if(!_states.ContainsKey(t))
		{
			_states.Add(t, state);
		}
		else
		{
			Debug.LogWarningFormat("Could not add state of type `{0}` because a state with that type already exists", t.ToString());
		}
	}

	private struct StateStatus
	{
		public Type StateType;
		public StateMachineState State;

		public bool IsValidState
		{
			get
			{
				return StateType != null && State != null;
			}
		}
	}
}

public abstract class StateMachineState
{
	public abstract void Activate();
	public abstract void Deactivate();
}