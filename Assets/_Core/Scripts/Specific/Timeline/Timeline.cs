using System;
using System.Collections.Generic;

public class Timeline : IReadableTimeline
{
	public delegate void TimelineEventHandler(TimelineEvent timelineEvent);
	public delegate void TimelineEventSuccessHandler(TimelineEvent timelineEvent, bool success);
	public event Action TimelineEndReachedEvent;
	public event Action TimelineStartReachedEvent;
	public event TimelineEventSuccessHandler TimelineEventEndedEvent;
	public event TimelineEventHandler TimelineUpwardsEvent;
	public event TimelineEventHandler TimelineDownwardsEvent;
	public event TimelineEventHandler NewTimelineEventEvent;

	public TimelineEvent[] Events
	{
		get
		{
			return _events.ToArray();
		}
	}

	public int TimelinePosition
	{
		get; private set;
	}

	private List<TimelineEvent> _events = new List<TimelineEvent>();

	public void EnqueueTimelineEvent(TimelineEvent timelineEvent)
	{
		_events.Add(timelineEvent);
	}

	public TimelineEvent GetCurrentTimelineEvent()
	{
		if(TimelinePosition < 0 || TimelinePosition >= _events.Count)
		{
			return null;
		}

		return _events[TimelinePosition];
	}

	public void Up()
	{
		if(TimelinePosition < _events.Count - 1)
		{
			SetCurrentTimelinePosition(++TimelinePosition);
			if(TimelineUpwardsEvent != null)
			{
				TimelineUpwardsEvent(GetCurrentTimelineEvent());
			}
		}
		else
		{
			if(TimelineEndReachedEvent != null)
			{
				TimelineEndReachedEvent();
			}
		}
	}

	public void Down()
	{
		if(TimelinePosition >= 0)
		{
			SetCurrentTimelinePosition(--TimelinePosition);
			if(TimelineDownwardsEvent != null)
			{
				TimelineDownwardsEvent(GetCurrentTimelineEvent());
			}
		}
		else
		{
			if(TimelineStartReachedEvent != null)
			{
				TimelineStartReachedEvent();
			}
		}
	}

	private void UnsetCurrentTimelineEvent()
	{
		TimelineEvent e = GetCurrentTimelineEvent();
		if(e != null && e.IsActive)
		{
			e.DeactivateEvent();
		}
	}

	private void SetCurrentTimelinePosition(int timelineIndex)
	{
		if(TimelinePosition == timelineIndex)
			return;

		UnsetCurrentTimelineEvent();
		TimelinePosition = timelineIndex;
		TimelineEvent currentEvent = GetCurrentTimelineEvent();

		if(currentEvent != null)
		{
			currentEvent.ActivateEvent(OnEventEndedCallback);

			if(NewTimelineEventEvent != null)
			{
				NewTimelineEventEvent(currentEvent);
			}
		}
	}

	private void OnEventEndedCallback(TimelineEvent timelineEvent, bool success)
	{
		if(GetCurrentTimelineEvent() == timelineEvent)
		{
			UnsetCurrentTimelineEvent();
			if(TimelineEventEndedEvent != null)
			{
				TimelineEventEndedEvent(timelineEvent, success);
			}
		}
	}
}


public interface IReadableTimeline
{
	int TimelinePosition
	{
		get;
	}

	TimelineEvent[] Events
	{
		get;
	}

	TimelineEvent GetCurrentTimelineEvent();
}