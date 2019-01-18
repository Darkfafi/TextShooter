using System;

public abstract class StateMachineState<T> : IStateMachineState<T> where T : class
{
	public enum StateMachineStateStatus
	{
		Initialized,
		Activated,
		Deactivated
	}

	public event Action<IStateMachineState<T>> StateInternallyEndedEvent;
	public event Action<IStateMachineState<T>, bool> CanBeInteruptedStateChangedEvent;

	public bool CanInteruptState
	{
		get; private set;
	}

	public T Affected
	{
		get; private set;
	}

	public StateMachineStateStatus Status
	{
		get; private set;
	}

	public StateMachineState()
	{
		CanInteruptState = true;
		Status = StateMachineStateStatus.Initialized;
	}

	public void Activate(T affected)
	{
		if(Status != StateMachineStateStatus.Initialized)
			return;

		Status = StateMachineStateStatus.Activated;
		Affected = affected;

		OnActivated();
	}

	public void Deactivate()
	{
		if(Status != StateMachineStateStatus.Activated)
			return;

		Status = StateMachineStateStatus.Deactivated;
		OnDeactivated();
		Affected = null;
	}

	protected void EndStateInternally()
	{
		Deactivate();

		CanInteruptState = false;
		SetCanBeInteruptedState(true);

		if(StateInternallyEndedEvent != null)
		{
			StateInternallyEndedEvent(this);
		}
	}

	protected void SetCanBeInteruptedState(bool canBeInterupted)
	{
		if(CanInteruptState == canBeInterupted)
			return;

		if(CanBeInteruptedStateChangedEvent != null)
		{
			CanBeInteruptedStateChangedEvent(this, CanInteruptState);
		}
	}

	protected abstract void OnActivated();
	protected abstract void OnDeactivated();
}

public interface IStateMachineState<T> where T : class
{
	event Action<IStateMachineState<T>> StateInternallyEndedEvent;
	event Action<IStateMachineState<T>, bool> CanBeInteruptedStateChangedEvent;

	bool CanInteruptState
	{
		get;
	}

	void Activate(T affected);
	void Deactivate();
}

public abstract class BaseStateMachineStateRequest<T, U> : IStateMachineStateRequest<U> where T : StateMachineState<U> where U : class
{
	public Type StateMachineStateType
	{
		get; private set;
	}

	public BaseStateMachineStateRequest()
	{
		StateMachineStateType = typeof(T);
	}

	public T CreateStateMachineState()
	{
		T state = Activator.CreateInstance<T>();
		SetupCreatedState(state);
		return state;
	}

	IStateMachineState<U> IStateMachineStateRequest<U>.CreateStateMachineState()
	{
		return CreateStateMachineState();
	}

	protected abstract void SetupCreatedState(T state);
}

public interface IStateMachineStateRequest<T> where T : class
{
	Type StateMachineStateType
	{
		get;
	}

	IStateMachineState<T> CreateStateMachineState();
}