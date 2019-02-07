public struct WeaponData : IStaticDatabaseData
{
	public string DataID
	{
		get; private set;
	}

	public int Damage
	{
		get; private set;
	}

	public float Radius
	{
		get; private set;
	}

	public bool HasCooldown
	{
		get; private set;
	}

	public float Cooldown
	{
		get; private set;
	}

	public WeaponData(string weaponID, int damage, float radius)
	{
		DataID = weaponID;
		Damage = damage;
		Radius = radius;
		HasCooldown = false;
		Cooldown = 0f;
	}

	public WeaponData(string weaponID, int damage, float radius, float cooldown)
	{
		DataID = weaponID;
		Damage = damage;
		Radius = radius;
		HasCooldown = cooldown > 0f;
		Cooldown = cooldown;
	}
}