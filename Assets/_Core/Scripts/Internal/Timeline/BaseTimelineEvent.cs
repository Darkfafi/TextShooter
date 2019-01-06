using System;
using System.Collections.Generic;

public abstract class BaseTimelineEvent<T, U> : ITimelineEvent where T : ITimelineEventData where U : class, IGame
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

	public void Setup(ITimelineState timelineState, ITimelineEventData data)
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
		EndEvent();
	}

	protected void EndEvent()
	{
		if(IsActive)
		{
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
	void Setup(ITimelineState timelineState, ITimelineEventData data);
}

public interface ITimelineEventData
{

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