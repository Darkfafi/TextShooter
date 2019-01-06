using System;

public class PotentialEvent
{
	public Type TimelineEventType
	{
		get; private set;
	}

	public ITimelineEventData Data
	{
		get; private set;
	}

	public static PotentialEvent Create<T, U, I>(I eventData) where T : class, IGame where U : BaseTimelineEvent<I, T> where I : ITimelineEventData
	{
		return new PotentialEvent(typeof(U), eventData);
	}

	public static PotentialEvent Create(Type eventType, ITimelineEventData eventData)
	{
		return new PotentialEvent(eventType, eventData);
	}

	private PotentialEvent(Type eventType, ITimelineEventData eventData)
	{
		TimelineEventType = eventType;
		Data = eventData;
	}
}