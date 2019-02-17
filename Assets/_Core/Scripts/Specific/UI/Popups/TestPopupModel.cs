public class TestPopupModel : BasePopupModel
{
	[PopupID]
	public const string POPUP_ID = "TestPopup";

	public override string PopupModelID
	{
		get
		{
			return POPUP_ID;
		}
	}

	public string TestText
	{
		get; private set;
	}

	public TestPopupModel(string testText)
	{
		TestText = testText;
	}
}
