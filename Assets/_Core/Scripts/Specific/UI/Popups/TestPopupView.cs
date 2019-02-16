using UnityEngine;
using UnityEngine.UI;

public class TestPopupView : MonoBaseView
{
	[SerializeField]
	private Text _testText;

	public void OnYesClicked()
	{
		MVCUtil.GetModel<TestPopupModel>(this).Close();
	}

	protected override void OnViewReady()
	{
		_testText.text = MVCUtil.GetModel<TestPopupModel>(this).TestText;
	}
}
