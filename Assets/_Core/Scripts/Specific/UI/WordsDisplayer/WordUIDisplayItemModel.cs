using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WordUIDisplayItemModel : EntityModel
{
	public EntityModel EntityModelLinkedTo;

	private TimekeeperModel _timekeeper;

	public WordUIDisplayItemModel(EntityModel entityModel, TimekeeperModel timekeeper)
	{
		EntityModelLinkedTo = entityModel;
		_timekeeper = timekeeper;

		_timekeeper.ListenToFrameTick(OnUpdate);
	}

	protected override void OnModelDestroy()
	{
		base.OnModelDestroy();
		_timekeeper.UnlistenFromFrameTick(OnUpdate);
		_timekeeper = null;
		EntityModelLinkedTo = null;
	}

	private void OnUpdate(float deltaTime, float timeScale)
	{
		ModelTransform.SetPos(EntityModelLinkedTo.ModelTransform.Position);
	}
}
