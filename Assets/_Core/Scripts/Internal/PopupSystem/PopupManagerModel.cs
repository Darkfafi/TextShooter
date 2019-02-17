using System;
using System.Collections.Generic;

public class PopupManagerModel : BaseModel
{
	public event Action<BasePopupModel> PopupStateChangedEvent;
	public event Action<BasePopupModel> OpenedPopupEvent;
	public event Action<BasePopupModel> PopupFocussedEvent;
	public event Action<BasePopupModel> PopupUnfocussedEvent;

	public BasePopupModel CurrentlyFocussedPopup
	{
		get; private set;
	}

	public int PopupsOpen
	{
		get
		{
			return _openPopups.Count;
		}
	}

	public int PopupsInQueue
	{
		get
		{
			return _requestedPopups.Count;
		}
	}

	private List<BasePopupModel> _openPopups = new List<BasePopupModel>();
	private List<BasePopupModel> _requestedPopups = new List<BasePopupModel>();

	public void RequestPopup(BasePopupModel popupModel, bool stack = false)
	{
		if(popupModel.PopupState != BasePopupModel.PopupModelState.InRequest)
		{
			UnityEngine.Debug.LogError("Trying to Request an already used popup. Request denied.");
			return;
		}

		_requestedPopups.Insert(stack ? 0 : _requestedPopups.Count, popupModel);
		if(_openPopups.Count == 0 || stack)
		{
			OpenNextRequest();
		}
	}

	public void CloseAll()
	{
		_requestedPopups.Clear();
		while(CurrentlyFocussedPopup != null)
		{
			CurrentlyFocussedPopup.Close();
		}
	}

	private void OpenNextRequest()
	{
		if(_requestedPopups.Count == 0)
			return;

		BasePopupModel nextPopup = _requestedPopups[0];
		_requestedPopups.Remove(nextPopup);
		nextPopup.PopupStateChangedEvent += OnPopupStateChangedEvent;
		_openPopups.Add(nextPopup);
		nextPopup.Open();
		RefreshFocus();

		if(OpenedPopupEvent != null)
		{
			OpenedPopupEvent(nextPopup);
		}
	}

	private void RefreshFocus()
	{
		BasePopupModel popupToFocus = null;
		if(_openPopups.Count > 0)
		{
			popupToFocus = _openPopups[_openPopups.Count - 1];
		}

		if(popupToFocus != CurrentlyFocussedPopup)
		{
			if(CurrentlyFocussedPopup != null)
				CurrentlyFocussedPopup.Unfocus();

			if(popupToFocus != null)
				popupToFocus.Focus();
		}
	}

	private void OnPopupStateChangedEvent(BasePopupModel popup)
	{
		switch(popup.PopupState)
		{
			case BasePopupModel.PopupModelState.Focussed:
				if(CurrentlyFocussedPopup != popup)
				{
					CurrentlyFocussedPopup = popup;
					if(PopupFocussedEvent != null)
					{
						PopupFocussedEvent(popup);
					}
				}
				break;
			case BasePopupModel.PopupModelState.Closed:
				_openPopups.Remove(popup);
				RefreshFocus();
				if(_openPopups.Count == 0)
					OpenNextRequest();
				break;
			case BasePopupModel.PopupModelState.Unfocussed:
				if(CurrentlyFocussedPopup == popup)
				{
					CurrentlyFocussedPopup = null;
					if(PopupUnfocussedEvent != null)
					{
						PopupUnfocussedEvent(popup);
					}
				}
				break;
		}

		if(PopupStateChangedEvent != null)
		{
			PopupStateChangedEvent(popup);
		}
	}
}
