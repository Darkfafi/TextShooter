public class WeaponHolder : BaseModelComponent
{
	public BaseWeapon Weapon
	{
		get; private set;
	}

	public bool HasWeapon
	{
		get
		{
			return Weapon != null;
		}
	}

	public void SetWeapon(BaseWeapon weapon)
	{
		BaseWeapon preWeapon = Weapon;

		if(preWeapon != null)
		{
			preWeapon.Clean();
		}

		Weapon = weapon;

		if(Weapon != null)
		{
			Weapon.SetHolder(Parent);
		}
	}

	protected override void Removed()
	{
		SetWeapon(null);
	}

	public bool UseWeaponIfAny(Lives livesComponent)
	{
		if(Weapon != null)
			return Weapon.Use(livesComponent);

		return false;
	}
}
