using UnityEngine;

public class EnemyFactory : IFactory<EnemyModel, EnemyFactoryData>
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

	public EnemyModel Create(EnemyFactoryData data)
	{
		EnemyData enemyData;

		if(!_enemyDatabase.TryGetData(data.EnemyID, out enemyData))
		{
			Debug.LogError("Could not find data for enemy with ID " + data.EnemyID);
			return null;
		}

		EnemyModel enemyModel;

		enemyModel = new EnemyModel(_timekeeperModel, data.EnemyPosition);
		enemyModel.Initialize(_wordsList.ListData.GetRandomWord(), _wordsList.ListData.GetRandomWords(enemyData.ExtraWordsAmount));

		ModelBrain<EntityModel> brain = enemyModel.AddComponent<EntityBrain>().Setup(_timekeeperModel);

		brain.SetupNoStateSwitcher(
			new MoveIntoRangeSwitcher(
			new MoveInRangeSwitcherData()
			{
				RangeToMoveTo = 1f,
				TargetFilterRules = FilterRules.CreateHasAllTagsFilter(Tags.ENEMY_TARGET),
				DistanceToTriggerSwitcher = 5f,
			})
		);

		brain.SetupNoStateSwitcher(
			new MoveIntoRangeSwitcher(
			new MoveInRangeSwitcherData() {
				RangeToMoveTo = 4f,
				TargetFilterRules = FilterRules.CreateHasAllTagsFilter(Tags.ENEMY_TARGET),
				SpecifiedSpeed = 2.25f,
			})
		);

		brain.SetupGlobalSwitcher(new UseWeaponInRangeSwitcher(FilterRules.CreateHasAllTagsFilter(Tags.ENEMY_TARGET), 0.8f));

		enemyModel.AddComponent<SuicideBombWeapon>();

		return enemyModel;
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

	public EnemyFactoryData(string enemyID, Vector3 enemyPosition)
	{
		EnemyID = enemyID;
		EnemyPosition = enemyPosition;
	}
}