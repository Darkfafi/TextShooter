﻿using UnityEngine;

public class EnemyFactory : IFactory<EnemyModel, EnemyFactoryData>
{
	private TimekeeperModel _timekeeperModel;

	public EnemyFactory(TimekeeperModel timekeeperModel)
	{
		_timekeeperModel = timekeeperModel;
	}

	~EnemyFactory()
	{
		_timekeeperModel = null;
	}

	public EnemyModel Create(EnemyFactoryData data)
	{
		EnemyModel enemyModel;

		switch(data.EnemyType)
		{
			// Suicide, RocketLauncher, Boss
			// Tank, RocketInfantry, SuicideInfantry, Plane, SuperTank.
			// Set Skin, Speed, damage and words
			// Get enemyType info from some xml?
			// Make Factory in GameModel's Factories? (Non static, not requiring TimekeeperModel parameter anymore)
		}

		enemyModel = new EnemyModel(_timekeeperModel, data.EnemyPosition);
		enemyModel.Initialize(data.EnemyType);
		EntityBrain brain = enemyModel.AddComponent<EntityBrain>();

		brain.SetupNoStateSwitcher(
			new MoveIntoRangeSwitcher(_timekeeperModel,
			new MoveInRangeSwitcherData()
			{
				RangeToMoveTo = 1f,
				TargetFilterRules = FilterRules.CreateHasAllTagsFilter(Tags.ENEMY_TARGET),
				DistanceToTriggerSwitcher = 3.5f,
			})
		);

		brain.SetupNoStateSwitcher(
			new MoveIntoRangeSwitcher(_timekeeperModel,
			new MoveInRangeSwitcherData() {
				RangeToMoveTo = 4f,
				TargetFilterRules = FilterRules.CreateHasAllTagsFilter(Tags.ENEMY_TARGET),
				SpecifiedSpeed = 2.25f,
			})
		);

		return enemyModel;
	}
}

public struct EnemyFactoryData : IFactoryData
{
	public string EnemyType
	{
		get; private set;
	}

	public Vector3 EnemyPosition
	{
		get; private set;
	}

	public EnemyFactoryData(string enemyType, Vector3 enemyPosition)
	{
		EnemyType = enemyType;
		EnemyPosition = enemyPosition;
	}
}