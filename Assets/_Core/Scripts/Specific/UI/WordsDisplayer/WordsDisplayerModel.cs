﻿using System;
using System.Collections.Generic;

public class WordsDisplayerModel : BaseModel
{
	public event Action<WordUIDisplayItemModel> AddedDisplayItemEvent;

	public EntityFilter<EntityModel> WordsHoldingEntities
	{
		get; private set;
	}

	private Dictionary<EntityModel, WordUIDisplayItemModel> _entitiesToWordUIDisplayItemMap = new Dictionary<EntityModel, WordUIDisplayItemModel>();

	private TimekeeperModel _timekeeper;

	public WordsDisplayerModel(TimekeeperModel timekeeper)
	{
		_timekeeper = timekeeper;

		FilterRules wordHoldingEntityFilter;
		FilterRules.OpenConstructHasAnyTags(Tags.DISPLAY_WORD);
		FilterRules.AddComponentToConstruct<WordsHolder>();
		FilterRules.CloseConstruct(out wordHoldingEntityFilter);

		WordsHoldingEntities = EntityFilter<EntityModel>.Create(wordHoldingEntityFilter);

		WordsHoldingEntities.TrackedEvent += OnTrackedEvent;
		WordsHoldingEntities.UntrackedEvent += OnUntrackedEvent;
	}

	public WordUIDisplayItemModel GetItemForEntityModel(EntityModel entityModel)
	{
		WordUIDisplayItemModel item = null;
		_entitiesToWordUIDisplayItemMap.TryGetValue(entityModel, out item);
		return item;
	}

	protected override void OnModelDestroy()
	{
		base.OnModelDestroy();

		foreach(var pair in _entitiesToWordUIDisplayItemMap)
		{
			pair.Value.Destroy();
		}

		_entitiesToWordUIDisplayItemMap.Clear();
		_entitiesToWordUIDisplayItemMap = null;

		WordsHoldingEntities.TrackedEvent -= OnTrackedEvent;
		WordsHoldingEntities.UntrackedEvent -= OnUntrackedEvent;

		WordsHoldingEntities.Clean();
		WordsHoldingEntities = null;
	}

	private void OnTrackedEvent(EntityModel entity)
	{
		if(!_entitiesToWordUIDisplayItemMap.ContainsKey(entity))
		{
			WordUIDisplayItemModel item = new WordUIDisplayItemModel(entity, _timekeeper);
			_entitiesToWordUIDisplayItemMap.Add(entity, item);
			if(AddedDisplayItemEvent != null)
			{
				AddedDisplayItemEvent(item);
			}
		}
	}

	private void OnUntrackedEvent(EntityModel entity)
	{
		WordUIDisplayItemModel item;
		if(_entitiesToWordUIDisplayItemMap.TryGetValue(entity, out item))
		{
			_entitiesToWordUIDisplayItemMap.Remove(entity);
			item.Destroy();
		}
	}
}
