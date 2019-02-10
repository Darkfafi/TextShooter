using System.Collections.Generic;

public abstract class BaseSwitcherDataObject<T> where T : class
{
	public PotentialSwitch<T> CreatePotentialSwitchToState(IStateMachineStateRequest<T> request, int priorityLevel, bool force = false, params KeyValuePair<string, bool>[] keysToSetOnSwitch)
	{
		return new PotentialSwitch<T>(request, priorityLevel, force, keysToSetOnSwitch);
	}

	public PotentialSwitch<T> CreatePotentialSwitchToNoState(int priorityLevel, bool force, params KeyValuePair<string, bool>[] keysToSetOnSwitch)
	{
		return new PotentialSwitch<T>(null, priorityLevel, force, keysToSetOnSwitch);
	}

	public virtual void Initialized()
	{
	}

	public virtual void Destroyed()
	{
	}
	public virtual void Activated()
	{
	}
	public virtual void Deactivated()
	{
	}
}