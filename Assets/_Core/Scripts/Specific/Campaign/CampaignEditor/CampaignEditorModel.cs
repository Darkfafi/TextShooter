using RDP.SaveLoadSystem;
using System;

namespace GameEditor
{
	public class CampaignEditorModel : BaseModel, IStorageCapsule
	{
		public event Action CampaignEditorLoadedEvent;

		public const string STORAGE_EDITOR_FILES_MANAGER_KEY = "EditorFilesManagerKey";

		public string ID
		{
			get
			{
				return StorageGlobals.CAPSULE_CAMPAIGN_EDITOR;
			}
		}

		public CampaignFilesManagerModel CampaignFilesManagerModel
		{
			get; private set;
		}

		public PopupManagerModel PopupManagerModel
		{
			get; private set;
		}

		private Storage _storage;

		public CampaignEditorModel()
		{
			PopupManagerModel = new PopupManagerModel();
			_storage = StorageGlobals.CreateStorage(this);
		}

		protected override void OnModelReady()
		{
			_storage.Load();
		}

		protected override void OnModelDestroy()
		{
			PopupManagerModel.Destroy();
			PopupManagerModel = null;

			CampaignFilesManagerModel.Destroy();
			CampaignFilesManagerModel = null;

			_storage.Save(true);
			_storage = null;
		}

		public void Save(IStorageSaver saver)
		{
			saver.SaveRef(STORAGE_EDITOR_FILES_MANAGER_KEY, CampaignFilesManagerModel);
		}

		public void Load(IStorageLoader loader)
		{
			loader.LoadRef<CampaignFilesManagerModel>(STORAGE_EDITOR_FILES_MANAGER_KEY, (instance) => CampaignFilesManagerModel = (instance == null ? new CampaignFilesManagerModel() : instance));
		}

		public void LoadingCompleted()
		{
			CampaignFilesManagerModel.SetupStorage(_storage);

			if(CampaignEditorLoadedEvent != null)
			{
				CampaignEditorLoadedEvent();
			}

			PopupManagerModel.RequestPopup(new EditorMenuPopupModel(CampaignFilesManagerModel, false));
		}
	}
}