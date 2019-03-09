using System;
using System.Collections.Generic;

namespace GameEditor
{
	public class EventsEditorModel : BaseModel
	{
		public event Action<CampaignEditorFile> CurrentCampaignEditorFileChangedEvent;

		public CampaignEditorFile CurrentCampaignEditorFile
		{
			get; private set;
		}

		public List<EventNodesSlotModel> EventNodeSlots
		{
			get
			{
				return CurrentCampaignEditorFile.EventNodesSlots;
			}
		}

		private CampaignFilesManager _campaignFilesManager;
		private PopupManagerModel _popupManagerModel;

		public EventsEditorModel(CampaignFilesManager campaignFilesManager, PopupManagerModel popupManagerModel)
		{
			_campaignFilesManager = campaignFilesManager;
			_popupManagerModel = popupManagerModel;
		}

		public EventNodesSlotModel AddEventNodesSlot()
		{
			if(CurrentCampaignEditorFile == null)
				throw new Exception("Can't add event without a CampaignEditorFile being loaded");

			EventNodesSlotModel model = new EventNodesSlotModel();
			model.Init(_popupManagerModel, CurrentCampaignEditorFile.CampaignEditorKeys);
			EventNodeSlots.Add(model);
			return model;
		}

		protected override void OnModelReady()
		{
			_campaignFilesManager.NewCurrentFileLoadedEvent += OnNewCurrentFileLoadedEvent;
			if(_campaignFilesManager.CurrentCampaignFile != null)
			{
				OnNewCurrentFileLoadedEvent(_campaignFilesManager.CurrentCampaignFile);
			}
		}

		protected override void OnModelDestroy()
		{
			CurrentCampaignEditorFile = null;
			_campaignFilesManager.NewCurrentFileLoadedEvent -= OnNewCurrentFileLoadedEvent;
			_campaignFilesManager = null;
		}

		private void OnNewCurrentFileLoadedEvent(CampaignEditorFile currentFile)
		{
			CurrentCampaignEditorFile = currentFile;

			if(CurrentCampaignEditorFile != null)
			{
				for(int i = 0, c = EventNodeSlots.Count; i < c; i++)
				{
					EventNodeSlots[i].Init(_popupManagerModel, CurrentCampaignEditorFile.CampaignEditorKeys);
				}
			}

			if(CurrentCampaignEditorFileChangedEvent != null)
			{
				CurrentCampaignEditorFileChangedEvent(CurrentCampaignEditorFile);
			}
		}
	}
}