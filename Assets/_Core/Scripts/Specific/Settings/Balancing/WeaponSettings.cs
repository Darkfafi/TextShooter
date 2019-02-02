using UnityEngine;

public class WeaponSettings : ISettings
{
	public const string DEFAULT_FOLDER = "Default/Balancing";
	public const string WEAPON_DATABASE_FILE_NAME = "weaponDatabase";

	public string WeaponDatabaseString
	{
		get; private set;
	}

	public WeaponSettings()
	{
		Reset();
	}

	public void Reset()
	{
		SetWeaponDatabaseStringToDefault();
	}

	public void SetWeaponDatabaseString(string weaponDatabaseString)
	{
		WeaponDatabaseString = weaponDatabaseString;
	}

	public void SetWeaponDatabaseStringToDefault()
	{
		SetWeaponDatabaseString(Resources.Load<TextAsset>(FileUtils.PathToFile(WEAPON_DATABASE_FILE_NAME, DEFAULT_FOLDER)).text);
	}
}
