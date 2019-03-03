using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameEditor
{
	public class EditorMenuPopupView : BasePopupView
	{
		[Header("Using Assets")]
		[SerializeField]
		private EditorSaveFileRow _saveFileRowPrefab;

		[Header("Links")]
		[SerializeField]
		private Transform _saveFileRowsHolder;

		[SerializeField]
		private Button _newButton;

		[SerializeField]
		private Button _exitButton;

		private EditorMenuPopupModel _editorPopupModel;
		private Dictionary<EditorSaveFileRow, CampaignEditorFile> _rowToFileMap = new Dictionary<EditorSaveFileRow, CampaignEditorFile>();

		protected override void OnViewReady()
		{
			_editorPopupModel = MVCUtil.GetModel<EditorMenuPopupModel>(this);

			_newButton.onClick.AddListener(_editorPopupModel.NewCampaignEditorFile);
			_exitButton.onClick.AddListener(_editorPopupModel.ExitEditorScreen);

			if(OptionalCloseButton != null)
				OptionalCloseButton.gameObject.SetActive(_editorPopupModel.CanClose);

			RefreshRows();
		}

		protected override void OnViewDestroy()
		{
			_newButton.onClick.RemoveAllListeners();
			_exitButton.onClick.RemoveAllListeners();

			_rowToFileMap.Clear();

			_rowToFileMap = null;
			_editorPopupModel = null;
		}

		private void CreateFileRows()
		{
			CampaignEditorFile[] files = _editorPopupModel.CampaignFilesHolder.CampaignFiles;
			for(int i = 0; i < files.Length; i++)
			{
				EditorSaveFileRow row = Instantiate(_saveFileRowPrefab, _saveFileRowsHolder);
				_rowToFileMap.Add(row, files[i]);
				CampaignEditorFileInfo fileInfo = _editorPopupModel.CampaignFilesHolder.GetFileInfoFor(files[i]);
				row.SetupRowItem(fileInfo.CampaignName, fileInfo.CampaignDescription, OnRowLoadButtonClicked, OnRowRemoveButtonClicked);
			}
		}

		private void RefreshRows()
		{
			if(_rowToFileMap.Count > 0)
			{
				foreach(var pair in _rowToFileMap)
				{
					Destroy(pair.Key.gameObject);
				}

				_rowToFileMap.Clear();
			}

			CreateFileRows();
		}

		private void OnRowLoadButtonClicked(EditorSaveFileRow row)
		{
			CampaignEditorFile fileLinkedToRow;
			if(_rowToFileMap.TryGetValue(row, out fileLinkedToRow))
			{
				_editorPopupModel.LoadFile(fileLinkedToRow);
			}
		}

		private void OnRowRemoveButtonClicked(EditorSaveFileRow row)
		{
			CampaignEditorFile fileLinkedToRow;
			if(_rowToFileMap.TryGetValue(row, out fileLinkedToRow))
			{
				_editorPopupModel.RemoveFile(fileLinkedToRow);
			}

			RefreshRows();
		}
	}
}