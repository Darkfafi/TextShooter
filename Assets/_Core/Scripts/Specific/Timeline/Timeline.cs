using System;
using System.Collections.Generic;

public class Timeline : IReadableTimeline
{
	public delegate void TimelineEventHandler(IReadableTimelineEvent timelineEvent);
	public delegate void TimelineEventSuccessHandler(IReadableTimelineEvent timelineEvent, bool success);
	public event Action TimelineEndReachedEvent;
	public event Action TimelineStartReachedEvent;
	public event TimelineEventSuccessHandler TimelineEventEndedEvent;
	public event TimelineEventHandler TimelineUpwardsEvent;
	public event TimelineEventHandler TimelineDownwardsEvent;
	public event TimelineEventHandler NewTimelineEventEvent;

	public IReadableTimelineEvent[] Events
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

	private List<ITimelineEvent> _events = new List<ITimelineEvent>();
	private TimekeeperModel _timekeeperModel;

	public Timeline(TimekeeperModel timekeeperModel)
	{
		_timekeeperModel = timekeeperModel;
		TimelinePosition = -1;
	}

	~Timeline()
	{
		UnsetCurrentTimelineEvent();
		_events.Clear();
		_events = null;
		_timekeeperModel = null;
	}

	public void EnqueueTimelineEvent<U, T>(T data) where U : TimelineEvent<T> where T : ITimelineEventData
	{
		EnqueueTimelineEvent<U, T>(data, new KeyValuePair<string, object>[] { });
	}

	public void EnqueueTimelineEvent<U, T>(T data, params KeyValuePair<string, object>[] keyValueParams) where U : TimelineEvent<T> where T : ITimelineEventData
	{
		TimelineEventParameters<T> timelineEventParams = new TimelineEventParameters<T>(data, keyValueParams);
		U timelineEvent = Activator.CreateInstance<U>();
		_events.Add(timelineEvent);
		timelineEvent.Initialize(_timekeeperModel, timelineEventParams);
	}

	public IReadableTimelineEvent GetCurrentTimelineEvent()
	{
		return GetCurrentEditableTimelineEvent();
	}

	public bool SetNewTimelinePosition(int timelineIndex)
	{
		if(TimelinePosition == timelineIndex)
			return false;

		UnsetCurrentTimelineEvent();
		TimelinePosition = timelineIndex;
		ITimelineEvent currentEvent = GetCurrentEditableTimelineEvent();

		if(currentEvent != null)
		{
			currentEvent.ActivateEvent(OnEventEndedCallback);

			if(NewTimelineEventEvent != null)
			{
				NewTimelineEventEvent(currentEvent);
			}

			return true;
		}

		return false;
	}

	public void Up()
	{
		if(TimelinePosition < _events.Count - 1)
		{
			if(SetNewTimelinePosition(TimelinePosition + 1))
			{
				if(TimelineUpwardsEvent != null)
				{
					TimelineUpwardsEvent(GetCurrentTimelineEvent());
				}
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
			if(SetNewTimelinePosition(TimelinePosition - 1))
			{
				if(TimelineDownwardsEvent != null)
				{
					TimelineDownwardsEvent(GetCurrentTimelineEvent());
				}
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

	private bool UnsetCurrentTimelineEvent()
	{
		ITimelineEvent e = GetCurrentEditableTimelineEvent();
		if(e != null && e.IsActive)
		{
			e.DeactivateEvent();
			return true;
		}

		return false;
	}

	private void OnEventEndedCallback(ITimelineEvent timelineEvent, bool success)
	{
		if(GetCurrentTimelineEvent() == timelineEvent)
		{
			if(UnsetCurrentTimelineEvent())
			{
				if(TimelineEventEndedEvent != null)
				{
					TimelineEventEndedEvent(timelineEvent, success);
				}
			}
		}
	}

	private ITimelineEvent GetCurrentEditableTimelineEvent()
	{
		if(TimelinePosition < 0 || TimelinePosition >= _events.Count)
		{
			return null;
		}

		return _events[TimelinePosition];
	}
}


public interface IReadableTimeline
{
	int TimelinePosition
	{
		get;
	}

	IReadableTimelineEvent[] Events
	{
		get;
	}

	IReadableTimelineEvent GetCurrentTimelineEvent();
}