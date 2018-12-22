using UnityEngine;

public class CharKeyboardInputView : MonoBaseView
{
	private CharInputModel _charInputModel;

	protected override void OnViewReady()
	{
		_charInputModel = MVCUtil.GetModel<CharInputModel>(this);
	}

	protected override void OnViewDestroy()
	{
		_charInputModel = null;
	}

	protected void Update()
	{
		string inputString = Input.inputString;
		if(inputString.Length > 0)
		{
			_charInputModel.DoCharInput(inputString[0]);
		}
	}
}
