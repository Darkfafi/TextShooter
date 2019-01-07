using System;
using System.Collections.Generic;

public class TimelineEventSlot<T> where T : class, IGame
{
	public PotentialEvent DefaultPotentialEvent
	{
		get; private set;
	}

	private List<ConditionalPotentialEventData> _conditionalPotentialEvents = new List<ConditionalPotentialEventData>();

	public TimelineEventSlot(PotentialEvent defaultPotentialEvent)
	{
		DefaultPotentialEvent = defaultPotentialEvent;
	}

	public void AddConditionalEvent(PotentialEvent potentialEvent, KeyValuePair<string, bool>[] keyConditions)
	{
		_conditionalPotentialEvents.Add(new ConditionalPotentialEventData(potentialEvent, keyConditions));
	}

	public ITimelineEvent CreateTimelineEvent(ITimelineState timelineState)
	{
		for(int i = 0; i < _conditionalPotentialEvents.Count; i++)
		{
			if(_conditionalPotentialEvents[i].IsValidToConditions(timelineState))
			{
				ITimelineEvent e = CreateTimelineEvent(timelineState, _conditionalPotentialEvents[i].PotentialEvent);
				return e;
			}
		}

		return CreateTimelineEvent(timelineState, DefaultPotentialEvent);
	}

	private ITimelineEvent CreateTimelineEvent(ITimelineState timelineState, PotentialEvent potentialEvent)
	{
		ITimelineEvent e = Activator.CreateInstance(potentialEvent.TimelineEventType) as ITimelineEvent;
		e.Setup(timelineState, potentialEvent.Data);
		return e;
	}

	private struct ConditionalPotentialEventData
	{
		public PotentialEvent PotentialEvent;
		public KeyValuePair<string, bool>[] KeyConditions;

		public ConditionalPotentialEventData(PotentialEvent potentialEvent, KeyValuePair<string, bool>[] keyConditions)
		{
			PotentialEvent = potentialEvent;
			KeyConditions = keyConditions;
		}

		public bool IsValidToConditions(ITimelineState timelineState)
		{
			foreach(var pair in KeyConditions)
			{
				if(timelineState.GetKey(pair.Key) != pair.Value)
				{
					return false;
				}
			}

			return true;
		}
	}
}
