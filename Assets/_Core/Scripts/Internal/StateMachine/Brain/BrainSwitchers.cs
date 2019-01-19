using System;

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
			if(_brainStateMachine == null)
				return null;
			return _brainStateMachine.Affected;
		}
	}

	public Type CurrentStateType
	{
		get
		{
			if(_brainStateMachine == null)
				return null;

			return _brainStateMachine.CurrentStateType;
		}
	}

	public bool IsCurrentStateValid
	{
		get
		{
			if(_brainStateMachine == null)
				return false;

			return _brainStateMachine.IsCurrentStateValid;
		}
	}

	private StateMachine<T> _brainStateMachine;

	public BaseBrainSwitcher()
	{
		SwitcherStatus = BrainSwitcherStatus.NotReady;
	}

	public void Initialize(StateMachine<T> stateMachine)
	{
		if(SwitcherStatus != BrainSwitcherStatus.NotReady)
			return;

		_brainStateMachine = stateMachine;
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
		_brainStateMachine = null;
	}

	public void Activate()
	{
		if(SwitcherStatus != BrainSwitcherStatus.Initialized && SwitcherStatus != BrainSwitcherStatus.Deactivated)
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
		if(_brainStateMachine != null)
		{
			_brainStateMachine.RequestState(request, force);
		}
	}

	public void RequestNoState(bool force)
	{
		if(_brainStateMachine != null)
		{
			_brainStateMachine.RequestNoState(force);
		}
	}

	protected abstract void Initialized();
	protected abstract void Destroyed();
	protected abstract void Activated();
	protected abstract void Deactivated();
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

	void Initialize(StateMachine<T> stateMachine);
	void Clean();
}
