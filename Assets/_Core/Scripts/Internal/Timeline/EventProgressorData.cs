using System.Collections.Generic;

public struct EventProgressorData
{
	public const int VALUE_AT_GOAL = -1337;

	public enum EventEndType
	{
		None = -1,
		AtHitValue = 1,
		AtGoalReach = 2
	}
	public string ProgressorName;
	public EventEndType EndEventType;
	public int ValueToSetKeyAt;
	public KeyValuePair<string, bool> KeyValuePairToSet;
	public string OptionalStringValue;
}
