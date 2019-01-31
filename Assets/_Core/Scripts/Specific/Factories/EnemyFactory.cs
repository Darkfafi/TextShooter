using UnityEngine;

public class EnemyFactory : IFactory<CharacterModel, EnemyFactoryData>
{
	private TimekeeperModel _timekeeperModel;
	private StaticDatabase<EnemyData> _enemyDatabase;
	private WordsList _wordsList;

	public EnemyFactory(TimekeeperModel timekeeperModel, StaticDatabase<EnemyData> enemyDatabase, WordsList wordsList)
	{
		_wordsList = wordsList;
		_timekeeperModel = timekeeperModel;
		_enemyDatabase = enemyDatabase;
	}

	~EnemyFactory()
	{
		_wordsList = null;
		_enemyDatabase = null;
		_timekeeperModel = null;
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
		ApplyWeapon(enemyData, enemyModel.AddComponent<WeaponHolder>());

		if(data.ApplyBrain)
		{
			ApplyBrain(enemyData, enemyModel);
		}

		return enemyModel;
	}

	private void ApplyWeapon(EnemyData enemyData, WeaponHolder weaponHolder)
	{
		switch(enemyData.WeaponType)
		{
			case "suicide":
				weaponHolder.SetWeapon(new SuicideBombWeapon(2f));
				break;
		}
	}

	private void ApplyBrain(EnemyData enemyData, CharacterModel enemyModel)
	{
		ModelBrain<EntityModel> brain = enemyModel.AddComponent<EntityBrain>().Setup(_timekeeperModel);

		brain.SetupNoStateSwitcher(
			new MoveIntoRangeSwitcher(
			new MoveInRangeSwitcherData()
			{
				RangeToMoveTo = 1f,
				TargetFilterRules = FilterRules.CreateHasAllTagsFilter(Tags.ENEMY_TARGET),
				DistanceToTriggerSwitcher = 5f,
				SpecifiedSpeed = enemyModel.TopDownMovement.BaseSpeed * 1.25f,
			})
		);

		brain.SetupNoStateSwitcher(
			new MoveIntoRangeSwitcher(
			new MoveInRangeSwitcherData()
			{
				RangeToMoveTo = 4f,
				TargetFilterRules = FilterRules.CreateHasAllTagsFilter(Tags.ENEMY_TARGET),
			})
		);

		brain.SetupGlobalSwitcher(new UseWeaponInRangeSwitcher(FilterRules.CreateHasAllTagsFilter(Tags.ENEMY_TARGET), 0.8f));
	}
}

public struct EnemyFactoryData : IFactoryData
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