using System;

public abstract class BaseTimelineEvent<T, U> : ITimelineEvent where T : ITimelineEventData where U : class, IGame
{
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

	private Action<ITimelineEvent> _eventEndedCallback;

	private bool _isDataSet = false;

	public void Setup(TimelineState<U> timelineState, T data)
	{
		if(_isDataSet)
			return;

		_isDataSet = true;
		TimelineState = timelineState;
		EventData = data;
	}

	public void Setup(ITimelineState timelineState, ITimelineEventData data)
	{
		Setup((TimelineState<U>)timelineState, (T)data);
	}

	~BaseTimelineEvent()
	{
		DeactivateEvent();
	}

	public void ActivateEvent(Action<ITimelineEvent> eventEndedCallback)
	{
		if(IsActive)
			return;

		_eventEndedCallback = eventEndedCallback;

		IsActive = true;
		EventActivated();
	}

	public void DeactivateEvent()
	{
		if(!IsActive)
			return;

		IsActive = false;
		EventDeactivated();
		_eventEndedCallback = null;
	}

	public abstract TimelineEventProgressor[] GetProgressors();

	protected void EndEvent()
	{
		if(IsActive)
		{
			_eventEndedCallback(this);
		}
	}

	protected abstract void EventActivated();
	protected abstract void EventDeactivated();
}


public interface ITimelineEvent : IReadableTimelineEvent
{
	void ActivateEvent(Action<ITimelineEvent> eventEndedCallback);
	void DeactivateEvent();
	void Setup(ITimelineState timelineState, ITimelineEventData data);
}

public interface ITimelineEventData
{

}

public interface IReadableTimelineEvent
{
	bool IsActive
	{
		get;
	}

	TimelineEventProgressor[] GetProgressors();
}