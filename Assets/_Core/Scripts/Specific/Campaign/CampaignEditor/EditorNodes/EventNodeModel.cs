using GameEditor.Data;
using Rules.Timeline;
using UnityEngine;
using RDP.SaveLoadSystem;

namespace GameEditor
{
	public class EventNodeModel : BaseModel, ISaveableLoad
	{
		public const string STORAGE_CURRENT_EVENT_MODEL_DATA_KEY = "CurrentEventModelDataKey";

		public EventNodeModelData CurrentEventModelData
		{
			get; private set;
		}

		public EventTypeRule SelectedEventTypeRules
		{
			get
			{
				EventTypeRule returnValue = default(EventTypeRule);
				if(_timelineRules == null)
					return returnValue;

				_timelineRules.TryGetEventTypeRule(CurrentEventModelData.EventType, out returnValue);

				return returnValue;
			}
		}

		public EventTypeRule[] AllEventTypeRules
		{
			get
			{
				if(_timelineRules == null)
					return new EventTypeRule[] { };

				return _timelineRules.EventTypes;
			}
		}

		private TimelineRules _timelineRules;

		public EventNodeModel()
		{
			_timelineRules = TimelineRules.GetRules();
		}

		public void SetEventNodeModelData(EventNodeModelData eventNodeModelData)
		{
			CurrentEventModelData = eventNodeModelData;
		}

		protected override void OnModelReady()
		{
			SelectEventTypeRule(_timelineRules.EventTypes[0]);
		}

		public void SelectEventTypeRule(EventTypeRule eventTypeRule)
		{
			SetEventNodeModelData(new EventNodeModelData(eventTypeRule.Type, eventTypeRule.EndingTypes[0].Name));
			Debug.Log(CurrentEventModelData.EventType + " <-- Selected");
		}

		protected override void OnModelDestroy()
		{
			_timelineRules = null;
		}

		public void Save(IStorageSaver saver)
		{
			saver.SaveStruct(STORAGE_CURRENT_EVENT_MODEL_DATA_KEY, CurrentEventModelData);
		}

		public void Load(IStorageLoader loader)
		{
			CurrentEventModelData = loader.LoadStruct<EventNodeModelData>(STORAGE_CURRENT_EVENT_MODEL_DATA_KEY);
		}

		public void LoadingCompleted()
		{

		}
	}
}