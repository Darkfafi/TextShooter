using UnityEngine;

public class WeaponFactory : IFactory<BaseWeapon, WeaponFactoryData>
{
	private StaticDatabase<WeaponData> _weaponDatabase;

	public void Setup(FactoryHolder factoryHolder)
	{

	}

	public WeaponFactory(StaticDatabase<WeaponData> weaponDatabase)
	{
		_weaponDatabase = weaponDatabase;
	}

	public BaseWeapon Create(WeaponFactoryData data)
	{
		WeaponData weaponData;
		if(data.OnlyID)
		{
			if(!_weaponDatabase.TryGetData(data.WeaponId, out weaponData))
			{
				Debug.LogError("Could not find data for weapon with ID " + data.WeaponId);
				return null;
			}
		}
		else
		{
			weaponData = data.WeaponData;
		}

		switch(weaponData.DataID)
		{
			case SuicideBombWeapon.ID_WEAPON:
				return new SuicideBombWeapon(weaponData.Radius, weaponData.Damage);

			default:
				Debug.LogError("No class to create linked to weapon ID " + weaponData.DataID);
				return null;
		}
	}
}

public struct WeaponFactoryData
{
	public bool OnlyID
	{
		get
		{
			return string.IsNullOrEmpty(WeaponId);
		}
	}

	public string WeaponId
	{
		get; private set;
	}

	public WeaponData WeaponData
	{
		get; private set;
	}

	public WeaponFactoryData(string id)
	{
		WeaponId = id;
		WeaponData = default(WeaponData);
	}

	public WeaponFactoryData(WeaponData weaponData)
	{
		WeaponData = weaponData;
		WeaponId = null;
	}
}
