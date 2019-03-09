namespace GameEditor
{
	public class CampaignEditorModel : BaseModel
	{
		public CampaignFilesManager CampaignFilesManager
		{
			get; private set;
		}

		public PopupManagerModel PopupManagerModel
		{
			get; private set;
		}

		public EventsEditorModel EventsEditorModel
		{
			get; private set;
		}

		public CampaignEditorModel()
		{
			PopupManagerModel = new PopupManagerModel();
			CampaignFilesManager = new CampaignFilesManager();
			EventsEditorModel = new EventsEditorModel(CampaignFilesManager);
		}

		protected override void OnModelReady()
		{
			PopupManagerModel.RequestPopup(new EditorMenuPopupModel(CampaignFilesManager, false));
		}

		protected override void OnModelDestroy()
		{
			EventsEditorModel.Destroy();
			EventsEditorModel = null;

			PopupManagerModel.Destroy();
			PopupManagerModel = null;

			CampaignFilesManager.SaveCurrentFile();
			CampaignFilesManager.Clean();
			CampaignFilesManager = null;
		}
	}
}