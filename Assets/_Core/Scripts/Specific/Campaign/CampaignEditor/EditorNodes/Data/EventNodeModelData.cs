using System;

namespace GameEditor.Data
{
	[Serializable]
	public struct EventNodeModelData
	{
		public string EventType;
		public ProgressorData[] ProgressorsData;
		public string EndingType;

		public EventNodeModelData(string eventType, string endingType, params ProgressorData[] progressorsData)
		{
			EventType = eventType;
			EndingType = endingType;
			ProgressorsData = progressorsData;
		}
	}

	[Serializable]
	public struct ProgressorData
	{
		public string ProgressorID;
		public string ProgressorParameterString;
	}

	[Serializable]
	public struct DataNodeData
	{
		public string NodeDataType;
		public DataNodeSectionData[] NodeDataSections;
	}

	[Serializable]
	public struct DataNodeSectionData
	{
		public string SectionType;
		public object Value;
	}
}