﻿using System.Collections.Generic;

public class TargetingWordItemModificationView : BaseView
{
	private TargetingWordItemModificationModel _targetingWordItemModificationModel;
	private List<WordUIDisplayItemView> _itemViewsModified = new List<WordUIDisplayItemView>();

	public override void SetupView(IMethodPermitter controller)
	{
		base.SetupView(controller);
		_targetingWordItemModificationModel = MVCUtil.GetModel<TargetingWordItemModificationModel>(this);
		_targetingWordItemModificationModel.TargetingSystemChangedEvent += OnTargetingSystemChangedEvent;
		_targetingWordItemModificationModel.CharAtItemIndexTypedEvent += OnWordItemTargetCharAttemptEvent;
	}

	protected override void OnViewDestroy()
	{
		_targetingWordItemModificationModel = null;
		_targetingWordItemModificationModel.TargetingSystemChangedEvent -= OnTargetingSystemChangedEvent;
		_targetingWordItemModificationModel.CharAtItemIndexTypedEvent -= OnWordItemTargetCharAttemptEvent;
	}

	private void OnTargetingSystemChangedEvent(TargetSystem targetSystem, TargetSystem oldTargetSystem)
	{
		if(oldTargetSystem != null)
		{
			List<EntityModel> oldTargetsToClean = oldTargetSystem.GetAllTargets(true);
			for(int i = 0; i < oldTargetsToClean.Count; i++)
			{
				ResetTextColor(_targetingWordItemModificationModel.GetWordUIItemForEntityModel(oldTargetsToClean[i]));
			}
		}

		if(targetSystem != null)
		{
			List<EntityModel> newTargetsToDisplay = targetSystem.GetAllTargets(false);
			for(int i = 0; i < newTargetsToDisplay.Count; i++)
			{
				WordUIDisplayItemModel item = _targetingWordItemModificationModel.GetWordUIItemForEntityModel(newTargetsToDisplay[i]);
				SetTextColor(item, item.CurrentlyDisplayingWord.Length - 1);
			}

			if(targetSystem.CurrentTypingTarget != null)
			{
				SetTextColor(_targetingWordItemModificationModel.GetWordUIItemForEntityModel(targetSystem.CurrentTypingTarget), targetSystem.CurrentShootIndex - 1);
			}
		}
	}

	private void SetTextColor(WordUIDisplayItemModel item, int index)
	{
		WordUIDisplayItemView itemView = MVCUtil.GetView<WordUIDisplayItemView>(item);
		itemView.SetTextColor(0, index, index == itemView.DisplayingWord.Length - 1 ? "00FF00" : "00AA00");
		if(!_itemViewsModified.Contains(itemView))
		{
			_itemViewsModified.Add(itemView);
			itemView.ViewDestroyedEvent += OnViewDestroyedEvent;
		}
	}

	private void ResetTextColor(WordUIDisplayItemModel item)
	{
		WordUIDisplayItemView itemView = MVCUtil.GetView<WordUIDisplayItemView>(item);
		itemView.ResetTextColor();
	}

	private void OnWordItemTargetCharAttemptEvent(WordUIDisplayItemModel item, int index)
	{
		SetTextColor(item, index);
	}

	private void OnViewDestroyedEvent(MonoBaseView itemView)
	{
		itemView.ViewDestroyedEvent -= OnViewDestroyedEvent;
		_itemViewsModified.Remove((WordUIDisplayItemView)itemView);
	}
}
