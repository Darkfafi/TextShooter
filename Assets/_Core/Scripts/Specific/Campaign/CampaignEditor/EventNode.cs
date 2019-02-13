using Rules.Timeline;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameEditor
{
	public class EventNode : MonoBehaviour
	{
		[SerializeField]
		private Dropdown _eventTypeDropdown;

		private TimelineRules _timelineRules;

		public void Awake()
		{
			_timelineRules = TimelineRules.GetRules();
			SetupEventTypeDropdown();
			OnEventTypeSelected();
		}

		public void OnEventTypeSelected()
		{
			SetupForEventType(_timelineRules.EventTypes[_eventTypeDropdown.value]);
		}

		private void SetupEventTypeDropdown()
		{
			List<string> _eventTypeOptions = new List<string>();
			for(int i = 0; i < _timelineRules.EventTypes.Length; i++)
			{
				_eventTypeOptions.Add(_timelineRules.EventTypes[i].Type);
			}

			_eventTypeDropdown.AddOptions(_eventTypeOptions);
		}

		private void SetupForEventType(EventTypeRule eventTypeRule)
		{
			Debug.Log(eventTypeRule.Type + " <-- Selected");
		}
	}
}