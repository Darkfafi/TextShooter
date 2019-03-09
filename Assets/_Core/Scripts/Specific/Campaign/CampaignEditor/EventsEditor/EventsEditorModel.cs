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

		public EventsEditorModel(CampaignFilesManager campaignFilesManager)
		{
			_campaignFilesManager = campaignFilesManager;
		}

		public EventNodesSlotModel AddEventNodesSlot()
		{
			EventNodesSlotModel model = new EventNodesSlotModel();
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

			if(CurrentCampaignEditorFileChangedEvent != null)
			{
				CurrentCampaignEditorFileChangedEvent(CurrentCampaignEditorFile);
			}
		}
	}
}