using System;

public abstract class TimelineEvent<T> : ITimelineEvent where T : ITimelineEventData
{
	public bool IsActive
	{
		get; private set;
	}

	public abstract int EventType
	{
		get;
	}

	public TimelineEventParameters<T> EventParams
	{
		get; private set;
	}

	protected TimekeeperModel timekeeperModel
	{
		get; private set;
	}

	private Action<ITimelineEvent, bool> _eventEndedCallback;

	public void Initialize(TimekeeperModel timekeeperModel, TimelineEventParameters<T> timelineEventParams)
	{
		if(this.timekeeperModel == null)
		{
			this.timekeeperModel = timekeeperModel;
			EventParams = timelineEventParams;
		}
	}

	~TimelineEvent()
	{
		DeactivateEvent();
		timekeeperModel = null;
	}

	public void ActivateEvent(Action<ITimelineEvent, bool> eventEndedCallback)
	{
		if(IsActive)
			return;

		_eventEndedCallback = eventEndedCallback;

		IsActive = true;
		EventActivated();

		timekeeperModel.ListenToFrameTick(EventTickUpdate);
	}

	public void DeactivateEvent()
	{
		if(!IsActive)
			return;

		IsActive = false;
		timekeeperModel.UnlistenFromFrameTick(EventTickUpdate);
		EventDeactivated();
		_eventEndedCallback = null;
	}

	public abstract TimelineEventProgressor[] GetProgressors();

	protected void EndEvent(bool successEnding)
	{
		if(IsActive)
		{
			_eventEndedCallback(this, successEnding);
		}
	}

	protected abstract void EventActivated();
	protected abstract void EventTickUpdate(float deltaTime, float timeScale);
	protected abstract void EventDeactivated();
}


public interface ITimelineEvent : IReadableTimelineEvent
{
	void ActivateEvent(Action<ITimelineEvent, bool> eventEndedCallback);
	void DeactivateEvent();
}

public interface IReadableTimelineEvent
{
	bool IsActive
	{
		get;
	}

	int EventType
	{
		get;
	}

	TimelineEventProgressor[] GetProgressors();
}