using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameFactories
{
	public EnemyFactory EnemyFactory
	{
		get; private set;
	}

	public GameFactories(GameModel game)
	{
		// Creation of Factories
		EnemyFactory = new EnemyFactory(game.TimekeeperModel);
	}

	~GameFactories()
	{
		EnemyFactory = null;
	}
}
