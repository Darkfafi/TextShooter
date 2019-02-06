﻿using UnityEngine;

public class EnemyFactory : IFactory<CharacterModel, EnemyFactoryData>
{
	private TimekeeperModel _timekeeperModel;
	private StaticDatabase<EnemyData> _enemyDatabase;
	private WordsList _wordsList;
	private WeaponFactory _weaponFactory;

	public EnemyFactory(TimekeeperModel timekeeperModel, StaticDatabase<EnemyData> enemyDatabase, WordsList wordsList)
	{
		_wordsList = wordsList;
		_timekeeperModel = timekeeperModel;
		_enemyDatabase = enemyDatabase;
	}

	public void Setup(FactoryHolder factoryHolder)
	{
		_weaponFactory = factoryHolder.GetFactory<WeaponFactory>();
	}

	~EnemyFactory()
	{
		_wordsList = null;
		_enemyDatabase = null;
		_timekeeperModel = null;
		_weaponFactory = null;
	}

	public CharacterModel Create(EnemyFactoryData data)
	{
		EnemyData enemyData;

		if(!_enemyDatabase.TryGetData(data.EnemyID, out enemyData))
		{
			Debug.LogError("Could not find data for enemy with ID " + data.EnemyID);
			return null;
		}

		CharacterModel enemyModel;

		enemyModel = new CharacterModel(_timekeeperModel, enemyData.MovementSpeed, data.EnemyPosition);
		enemyModel.Initialize(_wordsList.ListData.GetRandomWord(), _wordsList.ListData.GetRandomWords(enemyData.ExtraWordsAmount));
		enemyModel.AddComponent<WeaponHolder>().SetWeapon(_weaponFactory.Create(enemyData.WeaponData));

		if(data.ApplyBrain)
		{
			EnemyBrainCreator.ApplyBrain(enemyModel, enemyData, _timekeeperModel);
		}

		EnemyPassport passport = enemyModel.AddComponent<EnemyPassport>();
		passport.SetupPassport(enemyData);

		return enemyModel;
	}
}

public struct EnemyFactoryData
{
	public string EnemyID
	{
		get; private set;
	}

	public Vector3 EnemyPosition
	{
		get; private set;
	}

	public bool ApplyBrain
	{
		get; private set;
	}

	public EnemyFactoryData(string enemyID, Vector3 enemyPosition, bool applyBrain = true)
	{
		EnemyID = enemyID;
		EnemyPosition = enemyPosition;
		ApplyBrain = applyBrain;
	}
}