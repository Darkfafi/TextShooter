using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EnemyFactory
{
	public static EnemyModel CreateEnemy(TimekeeperModel timekeeperModel, string enemyType)
	{
		EnemyModel enemyModel;

		switch(enemyType)
		{
			// Suicide, RocketLauncher, Boss
			// Tank, RocketInfantry, SuicideInfantry, Plane, SuperTank.
			// Set Skin, Speed, damage and words
		}

		enemyModel = new EnemyModel(timekeeperModel, enemyType);
		return enemyModel;
	}
}
