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
			// Get enemyType info from some xml?
			// Make Factory in GameModel's Factories? (Non static, not requiring TimekeeperModel parameter anymore)
		}

		enemyModel = new EnemyModel(timekeeperModel, enemyType);
		return enemyModel;
	}
}
