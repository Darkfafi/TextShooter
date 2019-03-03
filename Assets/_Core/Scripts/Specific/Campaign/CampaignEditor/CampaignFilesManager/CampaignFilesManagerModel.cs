using System.Collections.Generic;
using RDP.SaveLoadSystem;

namespace GameEditor
{
	public class CampaignFilesManagerModel : BaseModel, ICampaignFilesHolder, ISaveableLoad
	{
		// Save / Load Keys
		public const string STORAGE_EDITOR_FILES_KEY = "EditorFilesKey";

		public CampaignEditorFile[] CampaignFiles
		{
			get
			{
				return _campaignEditorFiles.ToArray();
			}
		}

		public CampaignEditorFile CurrentCampaignFile
		{
			get; private set;
		}

		private List<CampaignEditorFile> _campaignEditorFiles = new List<CampaignEditorFile>();
		private Storage _editorStorage;

		public void SetupStorage(Storage storage)
		{
			_editorStorage = storage;
		}

		public void SaveCurrentFile()
		{
			if(CurrentCampaignFile != null && !_campaignEditorFiles.Contains(CurrentCampaignFile))
			{
				_campaignEditorFiles.Add(CurrentCampaignFile);
			}

			_editorStorage.Save(true);
		}

		public void LoadAsCurrentFile(CampaignEditorFile file)
		{
			SaveCurrentFile();
			CurrentCampaignFile = file;
		}

		public void RemoveFile(CampaignEditorFile file)
		{
			_campaignEditorFiles.Remove(file);
			if(file == CurrentCampaignFile)
			{
				LoadAsCurrentFile(new CampaignEditorFile());
			}
		}

		public void Save(IStorageSaver saver)
		{
			saver.SaveRefs(STORAGE_EDITOR_FILES_KEY, _campaignEditorFiles.ToArray());
		}

		public void Load(IStorageLoader loader)
		{
			loader.LoadRefs<CampaignEditorFile>(STORAGE_EDITOR_FILES_KEY, (refs) => _campaignEditorFiles = new List<CampaignEditorFile>(refs));
		}

		public void LoadingCompleted()
		{

		}
	}

	public interface ICampaignFilesHolder
	{
		CampaignEditorFile CurrentCampaignFile
		{
			get;
		}

		CampaignEditorFile[] CampaignFiles
		{
			get;
		}

		void SaveCurrentFile();
		void LoadAsCurrentFile(CampaignEditorFile file);
		void RemoveFile(CampaignEditorFile file);
	}
}