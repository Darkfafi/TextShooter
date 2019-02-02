using UnityEngine;

public class WeaponFactory : IFactory<BaseWeapon, WeaponData>
{
	private StaticDatabase<WeaponData> _weaponDatabase;

	public void Setup(FactoryHolder factoryHolder)
	{

	}

	public WeaponFactory(StaticDatabase<WeaponData> weaponDatabase)
	{
		_weaponDatabase = weaponDatabase;
	}

	public BaseWeapon Create(WeaponData data)
	{
		WeaponData weaponData;
		if(data.OnlyID)
		{
			if(!_weaponDatabase.TryGetData(data.DataID, out weaponData))
			{
				Debug.LogError("Could not find data for weapon with ID " + data.DataID);
				return null;
			}
		}
		else
		{
			weaponData = data;
		}

		switch(weaponData.DataID)
		{
			case SuicideBombWeapon.ID_WEAPON:
				return new SuicideBombWeapon(weaponData.Radius, weaponData.Damage);

			default:
				Debug.LogError("No class to create linked to weapon ID " + data.DataID);
				return null;
		}
	}
}
