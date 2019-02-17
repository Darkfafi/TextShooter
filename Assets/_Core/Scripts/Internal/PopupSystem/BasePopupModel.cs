using System;

public abstract class BasePopupModel : BaseModel
{
	public event Action<BasePopupModel> PopupStateChangedEvent;

	public enum PopupModelState
	{
		None,
		InRequest,
		Unfocussed,
		Focussed,
		Closed
	}

	public abstract string PopupModelID
	{
		get;
	}

	public PopupModelState PopupState
	{
		get
		{
			return _popupState;
		}
	}

	private PopupModelState _popupState = PopupModelState.None;

	public BasePopupModel()
	{
		SetPopupState(PopupModelState.InRequest);
	}

	public void Open()
	{
		if(PopupState != PopupModelState.InRequest)
			return;

		if(SetPopupState(PopupModelState.Unfocussed))
		{
			OnOpen();
		}
	}

	public void Focus()
	{
		if(PopupState != PopupModelState.Unfocussed)
			return;

		if(SetPopupState(PopupModelState.Focussed))
		{
			OnFocusChanged(true);
		}
	}

	public void Unfocus()
	{
		if(PopupState != PopupModelState.Focussed)
			return;

		if(SetPopupState(PopupModelState.Unfocussed))
		{
			OnFocusChanged(false);
		}
	}

	public void Close()
	{
		if(SetPopupState(PopupModelState.Closed))
		{
			OnClose();
			Clean();
		}
	}

	protected virtual void OnOpen()
	{

	}

	protected virtual void OnClose()
	{

	}

	protected virtual void OnFocusChanged(bool focus)
	{

	}

	protected virtual void Clean()
	{
		Destroy();
	}

	private bool SetPopupState(PopupModelState newState)
	{
		if(_popupState == newState)
			return false;

		_popupState = newState;

		if(PopupStateChangedEvent != null)
		{
			PopupStateChangedEvent(this);
		}

		return true;
	}
}

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
public class PopupIDAttribute : Attribute
{

}