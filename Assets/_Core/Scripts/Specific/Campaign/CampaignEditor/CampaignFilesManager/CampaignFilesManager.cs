using RDP.SaveLoadSystem;
using System;
using System.Collections.Generic;
using System.IO;

namespace GameEditor
{
	public class CampaignFilesManager : ICampaignFilesHolder
	{
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

		public CampaignFilesManager()
		{
			try
			{
				if(Directory.Exists(Storage.GetPathToStorage(StorageGlobals.STORAGE_LOCATION_CAMPAIGN_EDITOR_FILES)))
				{
					string[] foundFiles = Directory.GetFiles(Storage.GetPathToStorage(StorageGlobals.STORAGE_LOCATION_CAMPAIGN_EDITOR_FILES));
					for(int i = 0; i < foundFiles.Length; i++)
					{
						if(Path.GetExtension(foundFiles[i]) == "." + Storage.SAVE_FILE_EXTENSION)
						{
							_campaignEditorFiles.Add(new CampaignEditorFile(Path.GetFileNameWithoutExtension(foundFiles[i])));
						}
					}
				}
			}
			catch(Exception e)
			{
				UnityEngine.Debug.LogError("Failed to load editor files. Error: " + e.Message);
			}

			_editorStorage = StorageGlobals.CreateCampaignEditorStorage(_campaignEditorFiles.ToArray());
		}

		public void Clean()
		{
			_campaignEditorFiles.Clear();

			if(CurrentCampaignFile != null)
			{
				CurrentCampaignFile.Unload();
				CurrentCampaignFile = null;
			}

			_editorStorage = null;
		}

		public void Refresh()
		{
			_editorStorage.UpdateStorage(_campaignEditorFiles.ToArray());
		}

		public void SaveCurrentFile()
		{
			if(CurrentCampaignFile != null)
			{
				if(!_campaignEditorFiles.Contains(CurrentCampaignFile))
				{
					_campaignEditorFiles.Add(CurrentCampaignFile);
				}

				_editorStorage.UpdateStorage(_campaignEditorFiles.ToArray());
				_editorStorage.Save(true, CurrentCampaignFile.ID);
			}

			Refresh();
		}

		public void LoadAsCurrentFile(CampaignEditorFile file, bool saveBeforeLoad)
		{
			if(saveBeforeLoad)
				SaveCurrentFile();

			if(CurrentCampaignFile != null)
				CurrentCampaignFile.Unload();

			CurrentCampaignFile = file;

			_editorStorage.Load(CurrentCampaignFile.ID);
		}

		public void RemoveFile(CampaignEditorFile file)
		{
			_campaignEditorFiles.Remove(file);
			if(file == CurrentCampaignFile)
			{
				LoadAsCurrentFile(CampaignEditorFile.CreateNew(), false);
			}
			_editorStorage.Clear(true, file.ID);
			Refresh();
		}

		public CampaignEditorFileInfo GetFileInfoFor(CampaignEditorFile file)
		{
			ReadStorageResult result;
			_editorStorage.TryRead(file.ID, out result);
			return new CampaignEditorFileInfo(result.CapsuleStorage.GetValue(CampaignEditorFile.STORAGE_CAMPAIGN_NAME_KEY).ToString(), result.CapsuleStorage.GetValue(CampaignEditorFile.STORAGE_CAMPAIGN_DESCRIPTION_KEY).ToString());
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
		void LoadAsCurrentFile(CampaignEditorFile file, bool saveBeforeLoad);
		void RemoveFile(CampaignEditorFile file);
		void Refresh();
		CampaignEditorFileInfo GetFileInfoFor(CampaignEditorFile file);
	}

	public struct CampaignEditorFileInfo
	{
		public string CampaignName
		{
			get; private set;
		}

		public string CampaignDescription
		{
			get; private set;
		}

		public CampaignEditorFileInfo(string name, string description)
		{
			CampaignName = name;
			CampaignDescription = description;
		}
	}
}