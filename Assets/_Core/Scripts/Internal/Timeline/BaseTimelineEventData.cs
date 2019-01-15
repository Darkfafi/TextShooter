using System.Collections.Generic;

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

	public string EndingType
	{
		get; private set;
	}

	private List<KeyValuePair<string, bool>> _keysToSetStartEvent = new List<KeyValuePair<string, bool>>();
	private List<KeyValuePair<string, bool>> _keysToSetEndEvent = new List<KeyValuePair<string, bool>>();
	private List<EventProgressorData> _eventProgressorData = new List<EventProgressorData>();

	public void AddEventProgressorData(EventProgressorData eventProgressorData)
	{
		_eventProgressorData.Add(eventProgressorData);
	}

	public void SetEndingType(string endingType)
	{
		EndingType = endingType;
	}

	public EventProgressorData[] GetAllEventProgressorData()
	{
		return _eventProgressorData.ToArray();
	}

	public void AddKeyToSetAtStartEvent(string key, bool value)
	{
		_keysToSetStartEvent.Add(new KeyValuePair<string, bool>(key, value));
	}

	public void AddKeyToSetAtEndEvent(string key, bool value)
	{
		_keysToSetEndEvent.Add(new KeyValuePair<string, bool>(key, value));
	}
}