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
		get; private set;
	}

	public BaseBrainSwitcher()
	{
		SwitcherStatus = BrainSwitcherStatus.NotReady;
	}

	public void Initialize(T affected)
	{
		if(SwitcherStatus != BrainSwitcherStatus.NotReady)
			return;

		Affected = affected;
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

	void Initialize(T affected);
	void Clean();
}
