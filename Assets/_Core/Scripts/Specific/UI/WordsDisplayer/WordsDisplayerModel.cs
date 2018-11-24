using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WordsDisplayerModel : BaseModel
{
	private List<EntityModel> _displayingWordsHolders = new List<EntityModel>();
	private EntityFilter<EntityModel> _entities = EntityFilter<EntityModel>.Create(Tags.DISPLAY_WORD);

	private TimekeeperModel _timekeeper;

	public WordsDisplayerModel(TimekeeperModel timekeeper)
	{
		_timekeeper = timekeeper;
		_timekeeper.ListenToFrameTick(OnUpdate);
	}

	protected override void OnModelDestroy()
	{
		base.OnModelDestroy();
		_timekeeper.UnlistenFromFrameTick(OnUpdate);
	}

	private void OnUpdate(float deltaTime, float timeScale)
	{
		_entities.GetAll(
			(entity)=> 
			{
				return entity.GetComponent<WordsHolder>() != null;
			}
		);
	}
}
