using System;
using System.Collections.Generic;

public class Timeline<T> : IReadableTimeline<T> where T : class, IGame 
{
	public delegate void TimelineEventHandler(IReadableTimelineEvent timelineEvent);
	public event Action TimelineEndReachedEvent;
	public event Action TimelineStartReachedEvent;
	public event TimelineEventHandler TimelineEventEndedEvent;
	public event TimelineEventHandler TimelineUpwardsEvent;
	public event TimelineEventHandler TimelineDownwardsEvent;
	public event TimelineEventHandler NewTimelineEventEvent;

	public T Game
	{
		get; private set;
	}

	public TimelineState<T> TimelineState
	{
		get; private set;
	}

	public TimelineEventSlot<T>[] EventSlots
	{
		get
		{
			return _eventSlots.ToArray();
		}
	}

	public int TimelinePosition
	{
		get; private set;
	}

	public ITimelineEvent CurrentEvent
	{
		get; private set;
	}

	public TimelineEventSlot<T> CurrentEventSlot
	{
		get
		{
			if(TimelinePosition < 0 || TimelinePosition >= _eventSlots.Count)
			{
				return null;
			}

			return _eventSlots[TimelinePosition];
		}
	}

	private List<TimelineEventSlot<T>> _eventSlots = new List<TimelineEventSlot<T>>();
	private ITimelineEvent _currentEvent;

	public Timeline(T game, params TimelineEventSlot<T>[] timelineEventSlots)
	{
		Game = game;
		TimelineState = new TimelineState<T>(this);
		TimelinePosition = -1;
		EnqueueTimelineSlot(timelineEventSlots);
	}

	~Timeline()
	{
		UnsetCurrentTimelineEvent();
		_eventSlots.Clear();
		_eventSlots = null;
	}

	public void EnqueueTimelineSlot(PotentialEventSlot defaultPotentialEventSlot, params PotentialEventSlot[] conditionalPotentialEventSlot)
	{
		EnqueueTimelineSlot(new TimelineEventSlot<T>(defaultPotentialEventSlot, conditionalPotentialEventSlot));
	}

	public void EnqueueTimelineSlot(PotentialEventSlot defaultPotentialEventSlot)
	{
		EnqueueTimelineSlot(new TimelineEventSlot<T>(defaultPotentialEventSlot));
	}

	public void EnqueueTimelineSlot(params TimelineEventSlot<T>[] slots)
	{
		_eventSlots.AddRange(slots);
	}

	public bool SetNewTimelinePosition(int timelineIndex)
	{
		if(TimelinePosition == timelineIndex)
			return false;

		UnsetCurrentTimelineEvent();
		TimelinePosition = timelineIndex;

		if(CurrentEventSlot != null)
		{
			CurrentEvent = CurrentEventSlot.CreateTimelineEvent(TimelineState);
			CurrentEvent.ActivateEvent(OnEventEndedCallback);

			if(NewTimelineEventEvent != null)
			{
				NewTimelineEventEvent(CurrentEvent);
			}

			return true;
		}

		return false;
	}

	public void Up()
	{
		if(TimelinePosition < _eventSlots.Count - 1)
		{
			if(SetNewTimelinePosition(TimelinePosition + 1))
			{
				if(TimelineUpwardsEvent != null)
				{
					TimelineUpwardsEvent(CurrentEvent);
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
					TimelineDownwardsEvent(CurrentEvent);
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
		if(CurrentEvent != null && CurrentEvent.IsActive)
		{
			CurrentEvent.DeactivateEvent();
			return true;
		}

		return false;
	}

	private void OnEventEndedCallback(ITimelineEvent timelineEvent)
	{
		if(CurrentEvent == timelineEvent)
		{
			if(UnsetCurrentTimelineEvent())
			{
				if(TimelineEventEndedEvent != null)
				{
					TimelineEventEndedEvent(timelineEvent);
				}
			}
		}
	}
}


public interface IReadableTimeline<T> where T : class, IGame
{
	T Game
	{
		get;
	}

	TimelineState<T> TimelineState
	{
		get;
	}

	int TimelinePosition
	{
		get;
	}

	ITimelineEvent CurrentEvent
	{
		get;
	}

	TimelineEventSlot<T> CurrentEventSlot
	{
		get;
	} 

	TimelineEventSlot<T>[] EventSlots
	{
		get;
	}
}

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