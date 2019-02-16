public class CampaignEditorModel : BaseModel
{
	public PopupManagerModel PopupManagerModel
	{
		get; private set;
	}

	public CampaignEditorModel()
	{
		PopupManagerModel = new PopupManagerModel();
	}

	protected override void OnModelDestroy()
	{
		PopupManagerModel.Destroy();
		PopupManagerModel = null;
	}
}
