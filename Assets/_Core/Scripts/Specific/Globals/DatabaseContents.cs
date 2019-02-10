using UnityEngine;

public static class DatabaseContents
{
	public const string DEFAULT_FOLDER = "Default/Databases";
	public const string WEAPON_DATABASE_FILE_NAME = "weaponDatabase";
	public const string ENEMY_DATABASE_FILE_NAME = "enemyDatabase";

	public static string GetWeaponsDatabaseText()
	{
		return Resources.Load<TextAsset>(FileUtils.PathToFile(WEAPON_DATABASE_FILE_NAME, DEFAULT_FOLDER)).text;
	}

	public static string GetEnemyDatabaseText()
	{
		return Resources.Load<TextAsset>(FileUtils.PathToFile(ENEMY_DATABASE_FILE_NAME, DEFAULT_FOLDER)).text;
	}
}
