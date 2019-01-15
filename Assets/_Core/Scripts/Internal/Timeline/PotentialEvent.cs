using System;

public class PotentialEvent
{
	public Type TimelineEventType
	{
		get; private set;
	}

	public BaseTimelineEventData Data
	{
		get; private set;
	}

	public static PotentialEvent Create<T, U, I>(I eventData) where T : class, IGame where U : BaseTimelineEvent<I, T> where I : BaseTimelineEventData
	{
		return new PotentialEvent(typeof(U), eventData);
	}

	public static PotentialEvent Create(Type eventType, BaseTimelineEventData eventData)
	{
		return new PotentialEvent(eventType, eventData);
	}

	private PotentialEvent(Type eventType, BaseTimelineEventData eventData)
	{
		TimelineEventType = eventType;
		Data = eventData;
	}
}