using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rules.Timeline
{
	public class TimelineRules
	{
		public static TimelineRules GetRules()
		{
			if(_instance == null)
			{
				_instance = JsonUtility.FromJson<TimelineRules>(ResourceLocator.Locate<TextAsset>("timelineRules", "Rules").text);

				foreach(EventTypeRule rule in _instance.EventTypes)
				{
					_instance._typeRulesMap.Add(rule.Type, rule);
				}
			}

			return _instance;
		}

		private static TimelineRules _instance;

		public EventTypeRule[] EventTypes;
		private Dictionary<string, EventTypeRule> _typeRulesMap = new Dictionary<string, EventTypeRule>();

		private TimelineRules()
		{
		}

		public bool TryGetEventTypeRule(string eventType, out EventTypeRule eventTypeRule)
		{
			return _typeRulesMap.TryGetValue(eventType, out eventTypeRule);
		}
	}

	[Serializable]
	public struct EventTypeRule
	{
		public string Type;
		public ProgressorInfo[] Progressors;
		public EndingInfo[] EndingTypes;
		public DataNodeInfo[] DataNodes;
	}

	[Serializable]
	public struct ProgressorInfo
	{
		public string Name;
		public string Type;
		public string Description;
	}

	[Serializable]
	public struct EndingInfo
	{
		public string Name;
		public string Description;
	}

	[Serializable]
	public struct DataNodeInfo
	{
		public string Name;
		public DataSectionNodeInfo[] SectionNodes;
	}

	[Serializable]
	public struct DataSectionNodeInfo
	{
		public string Name;
		public string ValueType;
		public string ValueChoices;
		public string Description;
	}
}