using System;
using UnityEngine;

namespace Rules.Timeline
{
	public class TimelineRules
	{
		public EventTypeRule[] EventTypes;

		private static TimelineRules _instance;

		public static TimelineRules GetRules()
		{
			if(_instance == null)
			{
				_instance = JsonUtility.FromJson<TimelineRules>(ResourceLocator.Locate<TextAsset>("timelineRules", "Rules").text);
			}

			return _instance;
		}

		private TimelineRules()
		{

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