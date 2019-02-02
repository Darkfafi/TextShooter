using System;

public class WeaponHolder : BaseModelComponent
{
	public event Action<BaseWeapon> WeaponUsedEvent;

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
			preWeapon.WeaponUsedEvent -= OnWeaponUsedEvent;
			preWeapon.Clean();
		}

		Weapon = weapon;

		if(Weapon != null)
		{
			Weapon.SetHolder(Parent);
			Weapon.WeaponUsedEvent += OnWeaponUsedEvent;
		}
	}

	protected override void Removed()
	{
		SetWeapon(null);
	}

	public bool UseWeaponIfAny(Lives livesComponent)
	{
		if(Weapon != null && livesComponent != null)
		{
			return Weapon.Use(livesComponent);
		}

		return false;
	}

	private void OnWeaponUsedEvent(BaseWeapon weapon)
	{
		if(WeaponUsedEvent != null)
		{
			WeaponUsedEvent(weapon);
		}
	}

	[ModelEditorMethod]
	private void UseWeaponEditor(EntityModel entity)
	{
		UseWeaponIfAny(entity.GetComponent<Lives>());
	}
}
