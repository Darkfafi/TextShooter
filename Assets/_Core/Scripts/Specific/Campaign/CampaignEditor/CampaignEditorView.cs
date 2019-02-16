using UnityEngine;

public class CampaignEditorView : MonoBaseView
{
	[SerializeField]
	private MonoPopupManagerView _popupManagerView;

	private CampaignEditorModel _campaignEditorModel;

	private int _counter = 0;

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


		// Setup Campaign Editor
		Controller.Link(_campaignEditorModel, this);
	}

	protected void Update()
	{
		if(Input.GetKeyDown(KeyCode.Space))
		{
			_campaignEditorModel.PopupManagerModel.RequestPopup(new TestPopupModel("Cool Test Text #" + _counter), true);
			_counter++;
		}
	}
}
