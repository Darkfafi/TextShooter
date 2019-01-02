using System;

public abstract class TimelineEvent
{
	public bool IsActive
	{
		get; private set;
	}

	public abstract int EventType
	{
		get;
	}

	private Action<TimelineEvent, bool> _eventEndedCallback;

	public void ActivateEvent(Action<TimelineEvent, bool> eventEndedCallback)
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

	protected void EndEvent(bool successEnding)
	{
		_eventEndedCallback(this, successEnding);
	}

	protected abstract void EventActivated();
	protected abstract void EventDeactivated();
}
