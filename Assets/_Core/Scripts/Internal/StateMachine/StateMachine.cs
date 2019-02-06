using System;

public class StateMachine<T> : IStateMachine<T> where T : class
{
	public event Action<IStateMachineState<T>> StateInternallyEndedEvent;
	public event Action<IStateMachineState<T>> StateSetEvent;

	private IStateMachineState<T> _currentStateStatus;
	private IStateMachineStateRequest<T> _nextStateRequest;

	public Type CurrentStateType
	{
		get
		{
			if(_currentStateStatus == null)
				return null;

			return _currentStateStatus.GetType();
		}
	}

	public bool IsCurrentStateValid
	{
		get
		{
			return _currentStateStatus != null;
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
		SetToNoStateInternally(false);
		Affected = null;
	}

	public void RequestState(IStateMachineStateRequest<T> request, bool force = false)
	{
		if(!force)
		{
			RequestNextState(request);
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
			RequestNextState(null);
		}
	}

	private void SetToNoStateInternally(bool fireSetStateEvent)
	{
		if(_currentStateStatus != null)
		{
			_currentStateStatus.CanBeInteruptedStateChangedEvent -= OnCanBeInteruptedStateChangedEvent;
			_currentStateStatus.StateInternallyEndedEvent -= OnStateInternallyEndedEvent;
			_currentStateStatus.Deactivate();
		}

		_currentStateStatus = null;

		if(fireSetStateEvent)
		{
			if(StateSetEvent != null)
			{
				StateSetEvent(null);
			}
		}
	}

	private void RequestNextState(IStateMachineStateRequest<T> stateStatus)
	{
		_nextStateRequest = stateStatus;
		if(!TrySetNextState())
		{
			_currentStateStatus.CanBeInteruptedStateChangedEvent -= OnCanBeInteruptedStateChangedEvent;
			_currentStateStatus.CanBeInteruptedStateChangedEvent += OnCanBeInteruptedStateChangedEvent;
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
				_currentStateStatus = request.CreateStateMachineState();
				_currentStateStatus.StateInternallyEndedEvent += OnStateInternallyEndedEvent;
				_currentStateStatus.Activate(Affected);
				request.Clean();

				if(StateSetEvent != null)
				{
					StateSetEvent(_currentStateStatus);
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
		if(_currentStateStatus == null || _currentStateStatus.CanInteruptState)
		{
			IStateMachineStateRequest<T> req = _nextStateRequest;
			_nextStateRequest = null;
			SetStateInternally(req);
			return true;
		}

		return false;
	}

	private void OnCanBeInteruptedStateChangedEvent(IStateMachineState<T> state, bool canBeInterupted)
	{
		state.CanBeInteruptedStateChangedEvent -= OnCanBeInteruptedStateChangedEvent;
		if(state == _currentStateStatus)
			TrySetNextState();
	}

	private void OnStateInternallyEndedEvent(IStateMachineState<T> state)
	{
		TrySetNextState();
		if(StateInternallyEndedEvent != null)
			StateInternallyEndedEvent(state);
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