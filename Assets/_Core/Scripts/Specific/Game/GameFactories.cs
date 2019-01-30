using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameFactories
{
	public EnemyFactory EnemyFactory
	{
		get; private set;
	}

	public GameFactories(GameModel game, StaticDatabase<EnemyData> enemyDatabase, WordsList wordsList)
	{
		// Creation of Factories
		EnemyFactory = new EnemyFactory(game.TimekeeperModel, enemyDatabase, wordsList);
	}

	~GameFactories()
	{
		EnemyFactory = null;
	}
}
