using System.Collections.Generic;

public struct TimelineState<T> : ITimelineState where T : class, IGame
{
	public IReadableTimeline<T> Timeline
	{
		get; private set;
	}

	private Dictionary<string, bool> _timelineKeys;

	public TimelineState(Timeline<T> timeline)
	{
		_timelineKeys = new Dictionary<string, bool>();
		Timeline = timeline;
	}

	public void ResetState()
	{
		_timelineKeys.Clear();
	}

	public bool GetKey(string key)
	{
		bool val;
		if(_timelineKeys.TryGetValue(key, out val))
			return val;

		return false;
	}

	public void SetKey(string key, bool value)
	{
		if(_timelineKeys.ContainsKey(key))
		{
			_timelineKeys[key] = value;
		}
		else
		{
			_timelineKeys.Add(key, value);
		}
	}
}

public interface ITimelineState
{
	void ResetState();
	bool GetKey(string key);
	void SetKey(string key, bool value);
}