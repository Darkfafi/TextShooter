using System;
using System.Collections.Generic;

public abstract class BaseTimelineEvent<T, U> : ITimelineEvent where T : BaseTimelineEventData where U : class, IGame
{
	public event Action<ITimelineEvent> EventEndedEvent;

	public string UniqueEventId
	{
		get; private set;
	}

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

	public int ProgressorsToEndEvent
	{
		get; private set;
	}

	public bool HasEventEndingProgressors
	{
		get
		{
			return ProgressorsToEndEvent > 0;
		}
	}

	private List<BaseTimelineEventProgressor> _progressors = new List<BaseTimelineEventProgressor>();
	private bool _isDataSet = false;

	public void Setup(TimelineState<U> timelineState, T data)
	{
		if(_isDataSet)
			return;

		UniqueEventId = string.Concat(GetType().FullName, GetHashCode().ToString(), UnityEngine.Random.Range(0, 1338));
		ProgressorsToEndEvent = 0;
		_isDataSet = true;
		TimelineState = timelineState;
		EventData = data;

		PreActivate(timelineState, data);
		_progressors.Clear();

		BaseTimelineEventProgressor[] progressorsSupported = SetupProgressorsSupported();
		for(int i = 0; i < progressorsSupported.Length; i++)
		{
			if(EventData.IsProgressorToAdd(progressorsSupported[i].ProgressorName))
			{
				if(EventData.GetProgressorEventData(progressorsSupported[i].ProgressorName).EndEventType != BaseTimelineEventData.EventProgressorData.EventEndType.None)
				{
					ProgressorsToEndEvent++;
				}

				_progressors.Add(progressorsSupported[i]);
			}
		}
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
			_progressors[i].ProgressorValueUpdatedEvent += OnProgressorValueUpdatedEvent;
			_progressors[i].StartProgressor(EventData.GetProgressorEventData(_progressors[i].ProgressorName).OptionalStringValue);
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
			_progressors[i].ProgressorValueUpdatedEvent -= OnProgressorValueUpdatedEvent;
			_progressors[i].EndProgressor();
		}

		_progressors.Clear();

		EventDeactivated();
		EventEndedEvent = null;
	}

	private void OnProgressorValueUpdatedEvent(BaseTimelineEventProgressor progressor, int oldValue)
	{
		BaseTimelineEventData.EventProgressorData eventProgressorData = EventData.GetProgressorEventData(progressor.ProgressorName);
		if(eventProgressorData.ValueToSetKeyAt != BaseTimelineEventData.EventProgressorData.VALUE_AT_GOAL && eventProgressorData.ValueToSetKeyAt == progressor.CurrentValue)
		{
			if(!string.IsNullOrEmpty(eventProgressorData.KeyValuePairToSet.Key))
			{
				TimelineState.SetKey(eventProgressorData.KeyValuePairToSet.Key, eventProgressorData.KeyValuePairToSet.Value);
			}

			if(eventProgressorData.EndEventType == BaseTimelineEventData.EventProgressorData.EventEndType.AtHitValue)
			{
				EndEvent();
			}
		}
	}

	private void OnGoalMatchedEvent(BaseTimelineEventProgressor progressor)
	{
		BaseTimelineEventData.EventProgressorData eventProgressorData = EventData.GetProgressorEventData(progressor.ProgressorName);

		if(eventProgressorData.ValueToSetKeyAt == BaseTimelineEventData.EventProgressorData.VALUE_AT_GOAL)
		{
			if(!string.IsNullOrEmpty(eventProgressorData.KeyValuePairToSet.Key))
			{
				TimelineState.SetKey(eventProgressorData.KeyValuePairToSet.Key, eventProgressorData.KeyValuePairToSet.Value);
			}
		}

		if(eventProgressorData.EndEventType == BaseTimelineEventData.EventProgressorData.EventEndType.AtGoalReach)
		{
			EndEvent();
		}
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
	protected abstract BaseTimelineEventProgressor[] SetupProgressorsSupported();
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

	string UniqueEventId
	{
		get;
	}

	bool IsActive
	{
		get;
	}

	BaseTimelineEventProgressor[] GetProgressors();
}