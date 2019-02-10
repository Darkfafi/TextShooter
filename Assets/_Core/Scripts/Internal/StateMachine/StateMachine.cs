using System;

public class StateMachine<T> : IStateMachine<T> where T : class
{
	public event Action<IStateMachineState<T>> StateInternallyEndedEvent;
	public event Action<IStateMachineState<T>> StateSetEvent;

	private IStateMachineState<T> _currentState;
	private IStateMachineStateRequest<T> _nextStateRequest;

	public Type CurrentStateType
	{
		get
		{
			if(_currentState == null)
				return null;

			return _currentState.GetType();
		}
	}

	public bool IsCurrentStateValid
	{
		get
		{
			return _currentState != null;
		}
	}
	
	public bool CanSetNewStateImmediately
	{
		get
		{
			return _currentState == null || _currentState.CanInteruptState;
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

	public bool RequestState(IStateMachineStateRequest<T> request, bool force = false, bool setAsNextStateWhenNotSet = true)
	{
		return RequestNextState(request, setAsNextStateWhenNotSet, force);
	}

	public bool RequestNoState(bool force, bool setAsNextStateWhenNotSet = true)
	{
		return RequestNextState(null, setAsNextStateWhenNotSet, force);
	}

	private void SetToNoStateInternally(bool fireSetStateEvent)
	{
		if(_currentState != null)
		{
			_currentState.CanBeInteruptedStateChangedEvent -= OnCanBeInteruptedStateChangedEvent;
			_currentState.StateInternallyEndedEvent -= OnStateInternallyEndedEvent;
			_currentState.Deactivate();
		}

		_currentState = null;

		if(fireSetStateEvent)
		{
			if(StateSetEvent != null)
			{
				StateSetEvent(null);
			}
		}
	}

	private bool RequestNextState(IStateMachineStateRequest<T> stateStatus, bool setAsNextStateWhenNotSet, bool force)
	{
		_nextStateRequest = stateStatus;

		if(!TrySetNextState(force))
		{
			if(_currentState != null)
			{
				_currentState.CanBeInteruptedStateChangedEvent -= OnCanBeInteruptedStateChangedEvent;

				if(setAsNextStateWhenNotSet)
					_currentState.CanBeInteruptedStateChangedEvent += OnCanBeInteruptedStateChangedEvent;
			}
			
			if(!setAsNextStateWhenNotSet)
				_nextStateRequest = null;

			return false;
		}

		return true;
	}

	private void SetStateInternally(IStateMachineStateRequest<T> request)
	{
		bool willHaveNewState = request != null && request.IsAllowedToCreate();
		SetToNoStateInternally(!willHaveNewState);
		if(willHaveNewState)
		{
			if(request.IsAllowedToCreate())
			{
				_currentState = request.CreateStateMachineState();
				_currentState.StateInternallyEndedEvent += OnStateInternallyEndedEvent;
				_currentState.Activate(Affected);
				request.Clean();

				if(StateSetEvent != null)
				{
					StateSetEvent(_currentState);
				}
			}
		}
		else if(request != null)
		{
			request.Clean();
		}
	}

	private bool TrySetNextState(bool force = false)
	{
		if(CanSetNewStateImmediately || force)
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
		if(state == _currentState)
			TrySetNextState();
	}

	private void OnStateInternallyEndedEvent(IStateMachineState<T> state)
	{
		TrySetNextState(true);
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

	bool RequestState(IStateMachineStateRequest<T> request, bool force = false, bool setAsNextStateWhenNotSet = true);
	bool RequestNoState(bool force, bool setAsNextStateWhenNotSet = true);
}

public interface IStateMachineAffected
{

}