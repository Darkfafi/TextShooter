using UnityEngine;
using UnityEngine.UI;

namespace GameEditor
{
	public class EventsEditorView : MonoBaseView
	{
		[SerializeField]
		private Transform _eventNodeSlotsHolder;

		[SerializeField]
		private Button _addEventSlotButton;

		[SerializeField]
		private EventNodesSlotView _eventNodesSlotViewPrefab;

		private EventsEditorModel _eventsEditorModel;

		protected override void OnPreViewReady()
		{
			_eventsEditorModel = MVCUtil.GetModel<EventsEditorModel>(this);
			_eventsEditorModel.CurrentCampaignEditorFileChangedEvent += OnCurrentCampaignEditorFileChangedEvent;
		}

		protected override void OnViewReady()
		{
			_addEventSlotButton.onClick.AddListener(OnAddEventSlotButtonPressed);
		}

		protected override void OnViewDestroy()
		{
			_addEventSlotButton.onClick.RemoveListener(OnAddEventSlotButtonPressed);
			_eventsEditorModel.CurrentCampaignEditorFileChangedEvent -= OnCurrentCampaignEditorFileChangedEvent;
			_eventsEditorModel = null;
		}

		private void OnAddEventSlotButtonPressed()
		{
			CreateViewForEventNodeSlot(_eventsEditorModel.AddEventNodesSlot());
		}

		private void OnCurrentCampaignEditorFileChangedEvent(CampaignEditorFile file)
		{
			for(int i = 0; i < file.EventNodesSlots.Count; i++)
			{
				CreateViewForEventNodeSlot(file.EventNodesSlots[i]);
			}
		}

		private void CreateViewForEventNodeSlot(EventNodesSlotModel eventNodesSlotModel)
		{
			EventNodesSlotView view = Instantiate(_eventNodesSlotViewPrefab, _eventNodeSlotsHolder);
			Controller.Link(eventNodesSlotModel, view);
		}
	}
}