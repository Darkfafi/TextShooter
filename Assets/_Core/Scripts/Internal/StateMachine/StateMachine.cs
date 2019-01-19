using System;

public class StateMachine<T> : IStateMachine<T> where T : class
{
	public event Action<IStateMachineState<T>> StateInternallyEndedEvent;
	public event Action<IStateMachineState<T>> StateSetEvent;

	private StateStatus _currentStateStatus = new StateStatus();
	private StateStatus _nextStateStatus = new StateStatus();

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

	public T Affected
	{
		get; private set;
	}

	public StateMachine(T affected)
	{
		Affected = affected;
	}

	public void Clean()
	{
		_nextStateStatus = new StateStatus();
		SetToNoStateInternally(false);
		Affected = null;
	}

	public void RequestState(IStateMachineStateRequest<T> request, bool force = false)
	{
		if(!force)
		{
			RequestNextState(new StateStatus(request));
		}
		else
		{
			SetStateInternally(request);
		}
	}

	public void RequestNoState(bool force)
	{
		if(force)
		{
			SetToNoStateInternally(true);
		}
		else
		{
			RequestNextState(new StateStatus());
		}
	}

	private void SetToNoStateInternally(bool fireSetStateEvent)
	{
		if(_currentStateStatus.IsValidState)
		{
			_currentStateStatus.State.CanBeInteruptedStateChangedEvent -= OnCanBeInteruptedStateChangedEvent;
			_currentStateStatus.State.StateInternallyEndedEvent -= OnStateInternallyEndedEvent;
			_currentStateStatus.State.Deactivate();
			_currentStateStatus.StateRequest.Clean();
		}

		_currentStateStatus = default(StateStatus);

		if(fireSetStateEvent)
		{
			if(StateSetEvent != null)
			{
				StateSetEvent(null);
			}
		}
	}

	private void RequestNextState(StateStatus stateStatus)
	{
		_nextStateStatus = stateStatus;
		if(!TrySetNextState())
		{
			_currentStateStatus.State.CanBeInteruptedStateChangedEvent -= OnCanBeInteruptedStateChangedEvent;
			_currentStateStatus.State.CanBeInteruptedStateChangedEvent += OnCanBeInteruptedStateChangedEvent;
		}
	}

	private void SetStateInternally(IStateMachineStateRequest<T> request)
	{
		bool willHaveNewState = request != null && request.IsAllowedToCreate();
		SetToNoStateInternally(!willHaveNewState);
		if(willHaveNewState)
		{
			if(request.IsAllowedToCreate())
			{
				IStateMachineState<T> state = request.CreateStateMachineState();

				StateStatus newState = new StateStatus(request, state);
				state.StateInternallyEndedEvent += OnStateInternallyEndedEvent;

				_currentStateStatus = newState;
				_currentStateStatus.State.Activate(Affected);

				if(StateSetEvent != null)
				{
					StateSetEvent(_currentStateStatus.State);
				}
			}
		}
		else if(request != null)
		{
			request.Clean();
		}
	}

	private bool TrySetNextState()
	{
		if(!_currentStateStatus.IsValidState || _currentStateStatus.State.CanInteruptState)
		{
			StateStatus next = _nextStateStatus;
			_nextStateStatus = new StateStatus();
			SetStateInternally(next.StateRequest);
			return true;
		}

		return false;
	}

	private void OnCanBeInteruptedStateChangedEvent(IStateMachineState<T> state, bool canBeInterupted)
	{
		state.CanBeInteruptedStateChangedEvent -= OnCanBeInteruptedStateChangedEvent;
		if(state == _currentStateStatus.State)
			TrySetNextState();
	}

	private void OnStateInternallyEndedEvent(IStateMachineState<T> state)
	{
		TrySetNextState();
		if(StateInternallyEndedEvent != null)
			StateInternallyEndedEvent(state);
	}

	private struct StateStatus
	{
		public Type StateType
		{
			get
			{
				if(State == null)
					return null;

				return State.GetType();
			}
		}

		public IStateMachineStateRequest<T> StateRequest;
		public IStateMachineState<T> State;

		public bool IsValidState
		{
			get
			{
				return StateRequest != null && State != null;
			}
		}

		public StateStatus(IStateMachineStateRequest<T> request)
		{
			StateRequest = request;
			State = null;
		}

		public StateStatus(IStateMachineStateRequest<T> request, IStateMachineState<T> state)
		{
			StateRequest = request;
			State = state;
		}
	}
}

public interface IStateMachine<T> where T : class
{
	Type CurrentStateType
	{
		get;
	}

	bool IsCurrentStateValid
	{
		get;
	}

	T Affected
	{
		get;
	}

	void RequestState(IStateMachineStateRequest<T> request, bool force = false);
	void RequestNoState(bool force);
}

public interface IStateMachineAffected
{

}