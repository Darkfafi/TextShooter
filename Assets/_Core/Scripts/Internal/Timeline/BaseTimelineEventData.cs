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
	private Dictionary<string, List<EventProgressorData>> _progressorsToAdd = new Dictionary<string, List<EventProgressorData>>();

	public void AddProgressorByName(string progressorName, EventProgressorData eventProgressorData)
	{
		if(!_progressorsToAdd.ContainsKey(progressorName))
		{
			_progressorsToAdd.Add(progressorName, new List<EventProgressorData>());
		}

		_progressorsToAdd[progressorName].Add(eventProgressorData);
	}

	public bool IsProgressorToAdd(string progressorName)
	{
		return _progressorsToAdd.ContainsKey(progressorName);
	}

	public void SetEndingType(string endingType)
	{
		EndingType = endingType;
	}

	public EventProgressorData[] GetAllProgressorEventData(string progressorName)
	{
		List<EventProgressorData> progressorData;
		if(_progressorsToAdd.TryGetValue(progressorName, out progressorData))
			return progressorData.ToArray();

		return new EventProgressorData[] { };
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