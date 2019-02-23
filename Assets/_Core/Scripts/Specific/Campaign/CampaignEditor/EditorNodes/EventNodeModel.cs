using GameEditor.Data;
using Rules.Timeline;
using UnityEngine;

namespace GameEditor
{
	public class EventNodeModel : BaseModel
	{
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
	}
}