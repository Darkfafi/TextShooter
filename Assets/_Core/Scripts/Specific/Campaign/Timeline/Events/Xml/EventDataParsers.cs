public static class EventDataParsers
{
	public const string EVENT_TYPE_MOBS = "mobs";

	public static BaseTimelineEventDataParser GetDataParserForType(string eventType)
	{
		switch(eventType)
		{
			case EVENT_TYPE_MOBS:
				return new MobsDataParser();
		}

		return null;
	}
}
