using Rules.Timeline;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameEditor
{
	public class EventNodeView : MonoBaseView
	{
		[SerializeField]
		private Dropdown _eventTypeDropdown;

		private EventNodeModel _eventNodeModel;

		protected override void OnViewReady()
		{
			_eventNodeModel = MVCUtil.GetModel<EventNodeModel>(this);
			SetupEventTypeDropdown();
		}

		protected override void OnViewDestroy()
		{
			_eventNodeModel = null;
		}

		public void OnEventTypeSelected()
		{
			_eventNodeModel.SelectEventTypeRule(_eventNodeModel.AllEventTypeRules[_eventTypeDropdown.value]);
		}

		private void SetupEventTypeDropdown()
		{
			List<string> _eventTypeOptions = new List<string>();
			for(int i = 0; i < _eventNodeModel.AllEventTypeRules.Length; i++)
			{
				_eventTypeOptions.Add(_eventNodeModel.AllEventTypeRules[i].Type);
			}

			_eventTypeDropdown.AddOptions(_eventTypeOptions);
		}

		private void SetupEventNodeBody(EventTypeRule eventTypeRule)
		{

		}
	}
}