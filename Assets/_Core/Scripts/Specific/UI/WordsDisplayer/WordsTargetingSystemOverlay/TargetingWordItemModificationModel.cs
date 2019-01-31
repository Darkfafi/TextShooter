using System;

public class TargetingWordItemModificationModel : BaseModel
{
	public delegate void TargetWordUIItemHandler(WordUIDisplayItemModel item, int index);
	public event TargetWordUIItemHandler CharAtItemIndexTypedEvent;
	public event Action<Targeting, WordUIDisplayItemModel> RegisteredItemAddedEvent;
	public event Action<Targeting, WordUIDisplayItemModel> RegisteredItemRemovedEvent;
	public event Action<Targeting, bool> TargetingEnabledStateChangedEvent;

	private WordsDisplayerModel _wordsDisplayerModel;
	private Targeting _targeting;

	public TargetingWordItemModificationModel(Targeting targeting, WordsDisplayerModel wordsDisplayerModel)
	{
		_wordsDisplayerModel = wordsDisplayerModel;
		_wordsDisplayerModel.AddedDisplayItemEvent += OnAddedDisplayItemEvent;

		_targeting = targeting;
		_targeting.TargetCharTypedEvent += OnTargetCharTypedEvent;
		_targeting.TargetsFilter.TrackedEvent += OnTargetTrackedEvent;
		_targeting.TargetsFilter.UntrackedEvent += OnTargetUntrackedEvent;
		_targeting.TargetingEnabledStateChangedEvent += OnEnabledStateChangedEvent;
	}

	public WordUIDisplayItemModel GetWordUIItemForEntityModel(EntityModel entityModel)
	{
		if(_wordsDisplayerModel != null)
		{
			return _wordsDisplayerModel.GetItemForEntityModel(entityModel);
		}

		return null;
	}

	protected override void OnModelDestroy()
	{
		_wordsDisplayerModel.AddedDisplayItemEvent -= OnAddedDisplayItemEvent;
		_wordsDisplayerModel = null;

		_targeting.TargetCharTypedEvent -= OnTargetCharTypedEvent;
		_targeting.TargetsFilter.TrackedEvent -= OnTargetTrackedEvent;
		_targeting.TargetsFilter.UntrackedEvent -= OnTargetUntrackedEvent;
		_targeting.TargetingEnabledStateChangedEvent -= OnEnabledStateChangedEvent;
		_targeting = null;
	}

	private void OnAddedDisplayItemEvent(WordUIDisplayItemModel item)
	{
		if(_targeting != null)
		{
			if(_targeting.TargetsFilter.Has(item.EntityModelLinkedTo))
			{
				if(RegisteredItemAddedEvent != null)
				{
					RegisteredItemAddedEvent(_targeting, item);
				}
			}
		}
	}

	private void OnTargetTrackedEvent(EntityModel target)
	{
		if(_targeting != null)
		{
			if(RegisteredItemAddedEvent != null)
			{
				RegisteredItemAddedEvent(_targeting, GetWordUIItemForEntityModel(target));
			}
		}
	}

	private void OnTargetUntrackedEvent(EntityModel target)
	{
		if(_targeting != null)
		{
			if(RegisteredItemRemovedEvent != null)
			{
				RegisteredItemRemovedEvent(_targeting, GetWordUIItemForEntityModel(target));
			}
		}
	}

	private void OnTargetCharTypedEvent(EntityModel target, char newChar, char requiredChar, int index)
	{
		WordUIDisplayItemModel item = GetWordUIItemForEntityModel(target);
		if(item != null && WordsHolder.IsCharMatch(newChar, requiredChar))
		{
			if(CharAtItemIndexTypedEvent != null)
			{
				CharAtItemIndexTypedEvent(item, index);
			}
		}
	}

	private void OnEnabledStateChangedEvent(Targeting targeting, bool enabled)
	{
		if(TargetingEnabledStateChangedEvent != null)
		{
			TargetingEnabledStateChangedEvent(_targeting, enabled);
		}
	}
}