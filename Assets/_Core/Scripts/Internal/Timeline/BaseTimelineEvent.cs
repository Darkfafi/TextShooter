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

	private Dictionary<BaseTimelineEventProgressor, EventProgressorData> _progressorsToDataMap = new Dictionary<BaseTimelineEventProgressor, EventProgressorData>();
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
		_progressorsToDataMap.Clear();

		EventProgressorData[] progressorData = EventData.GetAllEventProgressorData();

		for(int i = 0; i < progressorData.Length; i++)
		{
			EventProgressorData currentProgressorData = progressorData[i];
			BaseTimelineEventProgressor progressor = CreateSupportedProgressor(currentProgressorData.ProgressorName);
			if(progressor != null)
			{
				_progressorsToDataMap.Add(progressor, currentProgressorData);

				if(currentProgressorData.EndEventType != EventProgressorData.EventEndType.None)
				{
					ProgressorsToEndEvent++;
				}
			}
		}
	}

	public BaseTimelineEventProgressor[] GetProgressors()
	{
		BaseTimelineEventProgressor[] progressors = new BaseTimelineEventProgressor[_progressorsToDataMap.Keys.Count];
		_progressorsToDataMap.Keys.CopyTo(progressors, 0);
		return progressors;
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

		foreach(var progressorPair in _progressorsToDataMap)
		{
			progressorPair.Key.GoalMatchedEvent += OnGoalMatchedEvent;
			progressorPair.Key.ProgressorValueUpdatedEvent += OnProgressorValueUpdatedEvent;
			progressorPair.Key.StartProgressor(progressorPair.Value.OptionalStringValue);
		}

		EventActivated();
	}

	public void DeactivateEvent()
	{
		if(!IsActive)
			return;

		IsActive = false;

		foreach(var progressorPair in _progressorsToDataMap)
		{
			progressorPair.Key.GoalMatchedEvent -= OnGoalMatchedEvent;
			progressorPair.Key.ProgressorValueUpdatedEvent -= OnProgressorValueUpdatedEvent;
			progressorPair.Key.EndProgressor();
		}

		_progressorsToDataMap.Clear();

		EventDeactivated();
		EventEndedEvent = null;
	}

	private void OnProgressorValueUpdatedEvent(BaseTimelineEventProgressor progressor, int oldValue)
	{
		if(_progressorsToDataMap.ContainsKey(progressor))
		{
			EventProgressorData eventProgressorData = _progressorsToDataMap[progressor];
			if(eventProgressorData.ValueToSetKeyAt != EventProgressorData.VALUE_AT_GOAL && eventProgressorData.ValueToSetKeyAt == progressor.CurrentValue)
			{
				if(!string.IsNullOrEmpty(eventProgressorData.KeyValuePairToSet.Key))
				{
					TimelineState.SetKey(eventProgressorData.KeyValuePairToSet.Key, eventProgressorData.KeyValuePairToSet.Value);
				}

				if(eventProgressorData.EndEventType == EventProgressorData.EventEndType.AtHitValue)
				{
					EndEvent();
				}
			}
		}
	}

	private void OnGoalMatchedEvent(BaseTimelineEventProgressor progressor)
	{
		if(_progressorsToDataMap.ContainsKey(progressor))
		{
			EventProgressorData eventProgressorData = _progressorsToDataMap[progressor];

			if(eventProgressorData.ValueToSetKeyAt == EventProgressorData.VALUE_AT_GOAL)
			{
				if(!string.IsNullOrEmpty(eventProgressorData.KeyValuePairToSet.Key))
				{
					TimelineState.SetKey(eventProgressorData.KeyValuePairToSet.Key, eventProgressorData.KeyValuePairToSet.Value);
				}
			}

			if(eventProgressorData.EndEventType == EventProgressorData.EventEndType.AtGoalReach)
			{
				EndEvent();
			}
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

			ExecuteEndingTypeEffect(EventData.EndingType);

			if(EventEndedEvent != null)
			{
				EventEndedEvent(this);
			}
		}
	}

	protected abstract void PreActivate(TimelineState<U> timelineState, T data);
	protected abstract void EventActivated();
	protected abstract BaseTimelineEventProgressor CreateSupportedProgressor(string progressorName);
	protected abstract void ExecuteEndingTypeEffect(string endingType);
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