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
			if(BrainStateMachine == null)
				return null;
			return BrainStateMachine.Affected;
		}
	}

	public StateMachine<T> BrainStateMachine
	{
		get; private set;
	}

	public BaseBrainSwitcher()
	{
		SwitcherStatus = BrainSwitcherStatus.NotReady;
	}

	public void Initialize(StateMachine<T> stateMachine)
	{
		if(SwitcherStatus != BrainSwitcherStatus.NotReady)
			return;

		BrainStateMachine = stateMachine;
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
		BrainStateMachine = null;
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
