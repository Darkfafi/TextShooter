using System.Collections.Generic;

public struct PotentialSwitch<T> where T : class
{
	public IStateMachineStateRequest<T> Request
	{
		get; private set;
	}

	public int PriorityLevel
	{
		get; private set;
	}

	public bool Force
	{
		get; private set;
	}

	public bool IsSet
	{
		get; private set;
	}

	public KeyValuePair<string, bool>[] KeysToSetOnSwitch
	{
		get; private set;
	}

	public PotentialSwitch(IStateMachineStateRequest<T> request, int priorityLevel, bool force = false, params KeyValuePair<string, bool>[] keysToSetOnSwitch)
	{
		Request = request;
		PriorityLevel = priorityLevel;
		IsSet = true;
		KeysToSetOnSwitch = keysToSetOnSwitch;
		Force = force;
	}

	public void Clean()
	{
		if(Request != null)
			Request.Clean();

		Request = null;
	}
}