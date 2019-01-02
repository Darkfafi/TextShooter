using System.Collections.Generic;

public struct TimelineEventParameters<T> where T : ITimelineEventData
{
	public T TimelineEventData
	{
		get; private set;
	}

	private Dictionary<string, object> _keyValueParameters;

	public TimelineEventParameters(T data, params KeyValuePair<string, object>[] keyValueParams)
	{
		TimelineEventData = data;
		_keyValueParameters = new Dictionary<string, object>();
		foreach(var kvp in keyValueParams)
		{
			_keyValueParameters.Add(kvp.Key, kvp.Value);
		}
	}

	public bool GetValue(string key, out bool value)
	{
		object v;
		_keyValueParameters.TryGetValue(key, out v);

		if(v is bool)
		{
			value = (bool)v;
			return true;
		}

		value = false;
		return false;
	}

	public bool GetValue(string key, out string value)
	{
		object v;
		_keyValueParameters.TryGetValue(key, out v);

		if(v is string)
		{
			value = (string)v;
			return true;
		}

		value = null;
		return false;
	}

	public bool GetValue(string key, out int value)
	{
		object v;
		_keyValueParameters.TryGetValue(key, out v);

		if(v is int)
		{
			value = (int)v;
			return true;
		}

		value = 0;
		return false;
	}

	public bool GetValue(string key, out string[] value)
	{
		object v;
		_keyValueParameters.TryGetValue(key, out v);

		if(v is string[])
		{
			value = (string[])v;
			return true;
		}

		value = new string[] { };
		return false;
	}
}

public interface ITimelineEventData
{
	
}