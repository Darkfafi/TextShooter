using UnityEngine;

public class EnemySettings : ISettings
{
	public const string DEFAULT_FOLDER = "Default/Balancing";
	public const string ENEMY_DATABASE_FILE_NAME = "enemyDatabase";

	public string EnemyDatabaseString
	{
		get; private set;
	}

	public EnemySettings()
	{
		Reset();
	}

	public void Reset()
	{
		SetEnemyDatabaseStringToDefault();
	}

	public void SetEnemyDatabaseString(string enemyDatabaseString)
	{
		EnemyDatabaseString = enemyDatabaseString;
	}

	public void SetEnemyDatabaseStringToDefault()
	{
		SetEnemyDatabaseString(Resources.Load<TextAsset>(FileUtils.PathToFile(ENEMY_DATABASE_FILE_NAME, DEFAULT_FOLDER)).text);
	}
}
