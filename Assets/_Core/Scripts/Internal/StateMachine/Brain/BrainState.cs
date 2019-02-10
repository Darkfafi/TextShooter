using System.Collections.Generic;

public struct BrainState<T> : IBrainState where T : class
{
	public IBrain<T> Brain
	{
		get; private set;
	}

	private Dictionary<string, bool> _brainKeys;

	public BrainState(Brain<T> brain)
	{
		_brainKeys = new Dictionary<string, bool>();
		Brain = brain;
	}

	public void ResetState()
	{
		_brainKeys.Clear();
	}

	public bool GetKey(string key)
	{
		bool val;
		if(_brainKeys.TryGetValue(key, out val))
			return val;

		return false;
	}

	public void SetKey(string key, bool value)
	{
		if(_brainKeys.ContainsKey(key))
		{
			_brainKeys[key] = value;
		}
		else
		{
			_brainKeys.Add(key, value);
		}
	}
}

public interface IBrainState
{
	void ResetState();
	bool GetKey(string key);
	void SetKey(string key, bool value);
}