using UnityEngine;

namespace GameEditor
{
	public class CampaignEditorView : MonoBaseView
	{
		[SerializeField]
		private MonoPopupManagerView _popupManagerView;

		[SerializeField]
		private EventsEditorView _eventsEditorView;

		private CampaignEditorModel _campaignEditorModel;

		protected void OnDestroy()
		{
			_campaignEditorModel.Destroy();
			_campaignEditorModel = null;
		}

		protected void Start()
		{
			_campaignEditorModel = new CampaignEditorModel();

			// Setup Popup Manager
			Controller.Link(_campaignEditorModel.PopupManagerModel, _popupManagerView);

			// Setup Event Nodes Editor
			Controller.Link(_campaignEditorModel.EventsEditorModel, _eventsEditorView);

			// Setup Campaign Editor
			Controller.Link(_campaignEditorModel, this);
		}
	}
}