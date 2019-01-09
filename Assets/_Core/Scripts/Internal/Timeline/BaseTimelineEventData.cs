﻿using System.Collections.Generic;

public abstract class BaseTimelineEventData
{
	public KeyValuePair<string, bool>[] KeysToSetStartEvent
	{
		get
		{
			return _keysToSetStartEvent.ToArray();
		}
	}

	public KeyValuePair<string, bool>[] KeysToSetEndEvent
	{
		get
		{
			return _keysToSetEndEvent.ToArray();
		}
	}

	public string CleanEventAtEndingType
	{
		get; private set;
	}

	private List<KeyValuePair<string, bool>> _keysToSetStartEvent = new List<KeyValuePair<string, bool>>();
	private List<KeyValuePair<string, bool>> _keysToSetEndEvent = new List<KeyValuePair<string, bool>>();
	private Dictionary<string, EventProgressorData> _progressorsToAdd = new Dictionary<string, EventProgressorData>();

	public void AddProgressorByName(string progressorName, EventProgressorData eventProgressorData)
	{
		if(!_progressorsToAdd.ContainsKey(progressorName))
			_progressorsToAdd.Add(progressorName, eventProgressorData);
	}

	public bool IsProgressorToAdd(string progressorName)
	{
		return _progressorsToAdd.ContainsKey(progressorName);
	}

	public void SetCleanEventAtEndingType(string cleanAtEndingType)
	{
		CleanEventAtEndingType = cleanAtEndingType;
	}

	public EventProgressorData GetProgressorEventData(string progressorName)
	{
		EventProgressorData progressorData;
		if(_progressorsToAdd.TryGetValue(progressorName, out progressorData))
			return progressorData;

		return progressorData;
	}

	public void AddKeyToSetAtStartEvent(string key, bool value)
	{
		_keysToSetStartEvent.Add(new KeyValuePair<string, bool>(key, value));
	}

	public void AddKeyToSetAtEndEvent(string key, bool value)
	{
		_keysToSetEndEvent.Add(new KeyValuePair<string, bool>(key, value));
	}

	public struct EventProgressorData
	{
		public const int VALUE_AT_GOAL = -1337;

		public enum EventEndType
		{
			None = -1,
			AtHitValue = 1,
			AtGoalReach = 2
		}

		public EventEndType EndEventType;
		public int ValueToSetKeyAt;
		public KeyValuePair<string, bool> KeyValuePairToSet;
		public string OptionalStringValue;
	}
}