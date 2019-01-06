using System;
using System.Collections.Generic;
using System.Xml;

public abstract class BaseTimelineEvent<T, U> : ITimelineEvent where T : BaseTimelineEventData where U : class, IGame
{
	public event Action<ITimelineEvent> EventEndedEvent;

	public bool IsActive
	{
		get; private set;
	}

	public TimelineState<U> TimelineState
	{
		get; private set;
	}

	public U Game
	{
		get
		{
			return TimelineState.Timeline.Game;
		}
	}

	public T EventData
	{
		get; private set;
	}

	public int ProgressorsInUse
	{
		get
		{
			return _progressors.Count;
		}
	}

	private List<BaseTimelineEventProgressor> _progressors = new List<BaseTimelineEventProgressor>();
	private bool _isDataSet = false;

	public void Setup(TimelineState<U> timelineState, T data)
	{
		if(_isDataSet)
			return;

		_isDataSet = true;
		TimelineState = timelineState;
		EventData = data;

		PreActivate(timelineState, data);
		_progressors.Clear();
		_progressors.AddRange(SetupProgressors(timelineState, data));
	}

	public BaseTimelineEventProgressor[] GetProgressors()
	{
		return _progressors.ToArray();
	}

	public void Setup(ITimelineState timelineState, BaseTimelineEventData data)
	{
		Setup((TimelineState<U>)timelineState, (T)data);
	}

	~BaseTimelineEvent()
	{
		DeactivateEvent();
	}

	public void ActivateEvent()
	{
		if(IsActive)
			return;

		IsActive = true;

		foreach(var pair in EventData.KeysToSetStartEvent)
		{
			TimelineState.SetKey(pair.Key, pair.Value);
		}

		for(int i = 0; i < _progressors.Count; i++)
		{
			_progressors[i].GoalMatchedEvent += OnGoalMatchedEvent;
			_progressors[i].StartProgressor();
		}

		EventActivated();
	}

	public void DeactivateEvent()
	{
		if(!IsActive)
			return;

		IsActive = false;

		for(int i = 0; i < _progressors.Count; i++)
		{
			_progressors[i].GoalMatchedEvent -= OnGoalMatchedEvent;
			_progressors[i].EndProgressor();
		}

		_progressors.Clear();

		EventDeactivated();
		EventEndedEvent = null;
	}

	private void OnGoalMatchedEvent(BaseTimelineEventProgressor progressor)
	{
		foreach(var pair in EventData.GetKeysToSetAfterProgress(progressor.ProgressorName))
		{
			TimelineState.SetKey(pair.Key, pair.Value);
		}

		EndEvent();
	}

	protected void EndEvent()
	{
		if(IsActive)
		{
			foreach(var pair in EventData.KeysToSetEndEvent)
			{
				TimelineState.SetKey(pair.Key, pair.Value);
			}

			if(EventEndedEvent != null)
			{
				EventEndedEvent(this);
			}
		}
	}

	protected abstract void PreActivate(TimelineState<U> timelineState, T data);
	protected abstract void EventActivated();
	protected abstract BaseTimelineEventProgressor[] SetupProgressors(TimelineState<U> timelineState, T data);
	protected abstract void EventDeactivated();
}


public interface ITimelineEvent : IReadableTimelineEvent
{
	void ActivateEvent();
	void DeactivateEvent();
	void Setup(ITimelineState timelineState, BaseTimelineEventData data);
}

public interface IReadableTimelineEvent
{
	event Action<ITimelineEvent> EventEndedEvent;
	
	bool IsActive
	{
		get;
	}

	BaseTimelineEventProgressor[] GetProgressors();
}

public abstract class BaseTimelineEventData
{
	public KeyValuePair<string, bool>[] KeysToSetStartEvent
	{
		get
		{
			return _keysToSetStartEvent.ToArray();
		}
	}

	public KeyValuePair<string, bool>[] KeysToSetEndEvent
	{
		get
		{
			return _keysToSetEndEvent.ToArray();
		}
	}

	private List<KeyValuePair<string, bool>> _keysToSetStartEvent = new List<KeyValuePair<string, bool>>();
	private List<KeyValuePair<string, bool>> _keysToSetEndEvent = new List<KeyValuePair<string, bool>>();
	private Dictionary<string, List<KeyValuePair<string, bool>>> _keysToSetAtEndOfProgressors = new Dictionary<string, List<KeyValuePair<string, bool>>>();

	public void AddKeyToSetAtStartEvent(string key, bool value)
	{
		_keysToSetStartEvent.Add(new KeyValuePair<string, bool>(key, value));
	}

	public void AddKeyToSetAtEndEvent(string key, bool value)
	{
		_keysToSetEndEvent.Add(new KeyValuePair<string, bool>(key, value));
	}

	public void AddKeyToSetAtEndOfProgressors(string progressName, string key, bool value)
	{
		if(_keysToSetAtEndOfProgressors.ContainsKey(progressName))
		{
			_keysToSetAtEndOfProgressors[progressName].Add(new KeyValuePair<string, bool>(key, value));
		}
		else
		{
			_keysToSetAtEndOfProgressors.Add(progressName, new List<KeyValuePair<string, bool>>() { new KeyValuePair<string, bool>(key, value) });
		}
	}

	public KeyValuePair<string, bool>[] GetKeysToSetAfterProgress(string progress)
	{
		List<KeyValuePair<string, bool>> keysToSet = new List<KeyValuePair<string, bool>>();
		if(!_keysToSetAtEndOfProgressors.TryGetValue(progress, out keysToSet))
			return new KeyValuePair<string, bool>[] { };

		return keysToSet.ToArray();
	}
}