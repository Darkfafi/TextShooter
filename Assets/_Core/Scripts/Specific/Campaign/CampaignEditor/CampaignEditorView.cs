using UnityEngine;

namespace GameEditor
{
	public class CampaignEditorView : MonoBaseView
	{
		[SerializeField]
		private MonoPopupManagerView _popupManagerView;

		[SerializeField]
		private CampaignFilesManagerView _campaignFilesManagerView;

		private CampaignEditorModel _campaignEditorModel;

		protected void OnDestroy()
		{
			_campaignEditorModel.CampaignEditorLoadedEvent -= OnCampaignEditorLoadedEvent;
			_campaignEditorModel.Destroy();
			_campaignEditorModel = null;
		}

		protected void Start()
		{
			_campaignEditorModel = new CampaignEditorModel();
			_campaignEditorModel.CampaignEditorLoadedEvent += OnCampaignEditorLoadedEvent;

			// Setup Campaign Editor
			Controller.Link(_campaignEditorModel, this);
		}

		private void OnCampaignEditorLoadedEvent()
		{
			_campaignEditorModel.CampaignEditorLoadedEvent -= OnCampaignEditorLoadedEvent;

			// Setup Popup Manager
			Controller.Link(_campaignEditorModel.PopupManagerModel, _popupManagerView);

			// Setup Campaign Files Manager
			Controller.Link(_campaignEditorModel.CampaignFilesManagerModel, _campaignFilesManagerView);
		}
	}
}