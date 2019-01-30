public struct EnemyData : IStaticDatabaseData
{
	public string DataID
	{
		get; private set;
	}

	public string EnemyType
	{
		get; private set;
	}

	public string WeaponType
	{
		get; private set;
	}

	public string BehaviourType
	{
		get; private set;
	}

	public int ExtraWordsAmount
	{
		get; private set;
	}

	public EnemyData(string dataID, string enemyType, string weaponType, string behaviourType, int extraWordsAmount = 0)
	{
		DataID = dataID;
		EnemyType = enemyType;
		WeaponType = weaponType;
		BehaviourType = behaviourType;
		ExtraWordsAmount = extraWordsAmount;
	}
}