﻿using UnityEngine;
using UnityEngine.UI;

public class MonoPopupManagerView : MonoBaseView
{
	public const string MONO_POPUP_VIEW_PREFAB_RESOURCE_LOCATION = "MonoPopupViews";

	[Header("Requirements")]
	[SerializeField]
	private Image _backdropImage;

	[SerializeField]
	private Transform _unfocussedPopupsHolder;

	[SerializeField]
	private Transform _focussedPopupHolder;

	private PopupManagerModel _popupManagerModel;

	public static MonoBaseView GetPopupViewPrefab(string popupID)
	{
		return ResourceLocator.Locate<MonoBaseView>(popupID, MONO_POPUP_VIEW_PREFAB_RESOURCE_LOCATION);
	}

	protected override void OnViewReady()
	{
		_popupManagerModel = MVCUtil.GetModel<PopupManagerModel>(this);
		_popupManagerModel.PopupFocussedEvent += OnPopupFocussedEvent;
		_popupManagerModel.PopupUnfocussedEvent += OnPopupUnfocussedEvent;
		_popupManagerModel.OpenedPopupEvent += OnOpenedPopupEvent;
		_popupManagerModel.PopupStateChangedEvent += OnPopupStateChangedEvent;
		RefreshBackdropImage();
	}

	protected override void OnViewDestroy()
	{
		_popupManagerModel.PopupFocussedEvent -= OnPopupFocussedEvent;
		_popupManagerModel.PopupUnfocussedEvent -= OnPopupUnfocussedEvent;
		_popupManagerModel.OpenedPopupEvent -= OnOpenedPopupEvent;
		_popupManagerModel.PopupStateChangedEvent -= OnPopupStateChangedEvent;
		_popupManagerModel = null;
	}

	private void CreateViewForPopup(BasePopupModel popup)
	{
		MonoBaseView popupViewPrefab = GetPopupViewPrefab(popup.PopupModelID);
		MonoBaseView popupViewInstance = Instantiate(popupViewPrefab);
		Controller.Link(popup, popupViewInstance);
	}

	private void OnOpenedPopupEvent(BasePopupModel popup)
	{
		CreateViewForPopup(popup);
		RefreshBackdropImage();
	}

	private void OnPopupFocussedEvent(BasePopupModel popup)
	{
		MVCUtil.GetView<MonoBaseView>(popup, (view) => 
		{
			if(view == null)
				return;

			view.transform.SetParent(_focussedPopupHolder, true);
		});
	}

	private void OnPopupUnfocussedEvent(BasePopupModel popup)
	{
		MVCUtil.GetView<MonoBaseView>(popup, (view) =>
		{
			if(view == null)
				return;

			view.transform.SetParent(_unfocussedPopupsHolder, true);
		});
	}

	private void OnPopupStateChangedEvent(BasePopupModel popup)
	{
		if(_popupManagerModel != null)
		{
			if(popup.PopupState == BasePopupModel.PopupModelState.Closed)
			{
				RefreshBackdropImage();
			}
		}
	}

	private void RefreshBackdropImage()
	{
		_backdropImage.gameObject.SetActive(_popupManagerModel != null && _popupManagerModel.PopupsOpen > 0);
	}
}
