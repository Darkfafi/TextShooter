using System;
using System.Collections.Generic;

public class TimelineEventSlot<T> where T : class, IGame
{
	public PotentialEventSlot DefaultPotentialEventData
	{
		get; private set;
	}

	public PotentialEventSlot[] ConditionalPotentialEventData
	{
		get; private set;
	}

	public TimelineEventSlot(PotentialEventSlot defaultPotentialEvent, params PotentialEventSlot[] conditionalPotentialEvents)
	{
		DefaultPotentialEventData = defaultPotentialEvent;
		ConditionalPotentialEventData = conditionalPotentialEvents;
	}

	public static PotentialEventSlot CreateDefaultPotentialEventSlot<U, I>(I eventData) where U : BaseTimelineEvent<I, T> where I : ITimelineEventData
	{
		return CreateDefaultPotentialEventSlot(typeof(U), eventData);
	}

	public static PotentialEventSlot CreateDefaultPotentialEventSlot(Type eventType, ITimelineEventData eventData)
	{
		return new PotentialEventSlot(eventType, eventData);
	}

	public static PotentialEventSlot CreateConditionalPotentialEventSlot<U, I>(I eventData, params KeyValuePair<string, bool>[] keyConditions) where U : BaseTimelineEvent<I, T> where I : ITimelineEventData
	{
		return CreateConditionalPotentialEventSlot(typeof(U), eventData, keyConditions);
	}

	public static PotentialEventSlot CreateConditionalPotentialEventSlot(Type eventType, ITimelineEventData eventData, params KeyValuePair<string, bool>[] keyConditions)
	{
		return new PotentialEventSlot(eventType, eventData, keyConditions);
	}

	public ITimelineEvent CreateTimelineEvent(ITimelineState timelineState)
	{
		for(int i = 0; i < ConditionalPotentialEventData.Length; i++)
		{
			if(ConditionalPotentialEventData[i].IsValidToConditions(timelineState))
			{
				ITimelineEvent e = CreateTimelineEvent(timelineState, ConditionalPotentialEventData[i]);
				return e;
			}
		}

		return CreateTimelineEvent(timelineState, DefaultPotentialEventData);
	}

	private ITimelineEvent CreateTimelineEvent(ITimelineState timelineState, PotentialEventSlot slot)
	{
		ITimelineEvent e = Activator.CreateInstance(slot.TimelineEventType) as ITimelineEvent;
		e.Setup(timelineState, slot.Data);
		return e;
	}
}

public struct PotentialEventSlot
{
	public Type TimelineEventType;
	public ITimelineEventData Data;
	public KeyValuePair<string, bool>[] KeyConditions;

	public PotentialEventSlot(Type eventType, ITimelineEventData eventData)
	{
		TimelineEventType = eventType;
		Data = eventData;
		KeyConditions = new KeyValuePair<string, bool>[] { };
	}

	public PotentialEventSlot(Type eventType, ITimelineEventData eventData, KeyValuePair<string, bool>[] keyConditions)
	{
		TimelineEventType = eventType;
		Data = eventData;
		KeyConditions = keyConditions;
	}

	public bool IsValidToConditions(ITimelineState timelineState)
	{
		if(KeyConditions == null)
			return true;

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
