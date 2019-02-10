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

	public WeaponData WeaponData
	{
		get; private set;
	}

	public float MovementSpeed
	{
		get; private set;
	}

	public int ExtraWordsAmount
	{
		get; private set;
	}

	public EnemyData(string dataID, string enemyType, WeaponData weaponData, float movementSpeed, int extraWordsAmount = 0)
	{
		DataID = dataID;
		MovementSpeed = movementSpeed;
		EnemyType = enemyType;
		WeaponData = weaponData;
		ExtraWordsAmount = extraWordsAmount;
	}
}