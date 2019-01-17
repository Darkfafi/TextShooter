using System;
using System.Collections.Generic;

public class TargetingWordItemModificationView : BaseView
{
	private TargetingWordItemModificationModel _targetingWordItemModificationModel;
	private List<WordUIDisplayItemView> _itemViewsModified = new List<WordUIDisplayItemView>();

	public override void SetupView(IMethodPermitter controller)
	{
		base.SetupView(controller);
		_targetingWordItemModificationModel = MVCUtil.GetModel<TargetingWordItemModificationModel>(this);
		_targetingWordItemModificationModel.TargetingEnabledStateChangedEvent += OnTargetingEnabledStateChangedEvent;
		_targetingWordItemModificationModel.CharAtItemIndexTypedEvent += OnCharAtItemIndexTypedEvent;
		_targetingWordItemModificationModel.RegisteredItemAddedEvent += OnRegisteredItemAddedEvent;
		_targetingWordItemModificationModel.RegisteredItemRemovedEvent += OnRegisteredItemRemovedEvent;
	}

	protected override void OnViewDestroy()
	{
		_targetingWordItemModificationModel = null;
		_targetingWordItemModificationModel.TargetingEnabledStateChangedEvent -= OnTargetingEnabledStateChangedEvent;
		_targetingWordItemModificationModel.CharAtItemIndexTypedEvent -= OnCharAtItemIndexTypedEvent;
		_targetingWordItemModificationModel.RegisteredItemAddedEvent -= OnRegisteredItemAddedEvent;
		_targetingWordItemModificationModel.RegisteredItemRemovedEvent -= OnRegisteredItemRemovedEvent;
	}

	private void OnTargetingEnabledStateChangedEvent(Targeting targeting, bool enabledState)
	{
		if(targeting != null)
		{
			List<EntityModel> oldTargetsToClean = targeting.GetAllTargets(true);
			for(int i = 0; i < oldTargetsToClean.Count; i++)
			{
				ResetTextColor(_targetingWordItemModificationModel.GetWordUIItemForEntityModel(oldTargetsToClean[i]));
			}
		}

		if(targeting != null && enabledState)
		{
			ApplyStylingForTargets(targeting, targeting.GetAllTargets(true).ToArray());
		}
	}

	private void OnRegisteredItemAddedEvent(Targeting targeting, WordUIDisplayItemModel item)
	{
		if(item != null && !item.IsDestroyed)
		{
			ApplyStylingForTargets(targeting, item.EntityModelLinkedTo);
		}
	}

	private void OnRegisteredItemRemovedEvent(Targeting targeting, WordUIDisplayItemModel item)
	{
		if(item != null && !item.IsDestroyed)
		{
			ResetTextColor(item);
		}
	}

	private void ApplyStylingForTargets(Targeting targeting, params EntityModel[] targets)
	{
		for(int i = 0; i < targets.Length; i++)
		{
			if(targeting.IsTargetCompleted(targets[i]))
			{
				WordUIDisplayItemModel item = _targetingWordItemModificationModel.GetWordUIItemForEntityModel(targets[i]);
				SetTextColor(item, item.CurrentlyDisplayingWord.Length - 1);
			}
			else if(targets[i] == targeting.CurrentTypingTarget)
			{
				SetTextColor(_targetingWordItemModificationModel.GetWordUIItemForEntityModel(targeting.CurrentTypingTarget), targeting.CurrentShootIndex - 1);
			}
		}
	}

	private void SetTextColor(WordUIDisplayItemModel item, int index)
	{
		MVCUtil.GetView<WordUIDisplayItemView>(item, (itemView) => 
		{
			itemView.SetTextColor(0, index, index == itemView.DisplayingWord.Length - 1 ? "00FF00" : "00AA00");
			if(!_itemViewsModified.Contains(itemView))
			{
				_itemViewsModified.Add(itemView);
				itemView.ViewDestroyedEvent += OnViewDestroyedEvent;
			}
		});
	}

	private void ResetTextColor(WordUIDisplayItemModel item)
	{
		MVCUtil.GetView<WordUIDisplayItemView>(item, (itemView)=>
		{
			itemView.ResetTextColor();
		});
	}

	private void OnCharAtItemIndexTypedEvent(WordUIDisplayItemModel item, int index)
	{
		SetTextColor(item, index);
	}

	private void OnViewDestroyedEvent(MonoBaseView itemView)
	{
		itemView.ViewDestroyedEvent -= OnViewDestroyedEvent;
		_itemViewsModified.Remove((WordUIDisplayItemView)itemView);
	}
}
