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

		enemyModel = new EnemyModel(_timekeeperModel, data.EnemyType);
		return enemyModel;
	}
}

public struct EnemyFactoryData : IFactoryData
{
	public string EnemyType
	{
		get; private set;
	}

	public EnemyFactoryData(string enemyType)
	{
		EnemyType = enemyType;
	}
}