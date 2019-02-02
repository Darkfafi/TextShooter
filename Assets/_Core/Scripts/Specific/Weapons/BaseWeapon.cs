using System;

public abstract class BaseWeapon
{
	public event Action<BaseWeapon> WeaponUsedEvent;

	public int Damage
	{
		get; private set;
	}

	public float Radius
	{
		get; private set;
	}

	public abstract bool CanBeUsed
	{
		get;
	}

	public BaseModel Holder
	{
		get; private set;
	}

	public BaseWeapon(float radius, int damage = 1)
	{
		SetDamage(damage);
		SetRadius(radius);
	}

	public virtual void Clean()
	{
		SetHolder(null);
	}

	public void SetHolder(BaseModel holder)
	{
		Holder = holder;
	}

	public bool Use(Lives livesComponent)
	{
		if(CanBeUsed)
		{
			if(OnUse(livesComponent))
			{
				if(WeaponUsedEvent != null)
				{
					WeaponUsedEvent(this);
				}

				return true;
			}
		}

		return false;
	}

	public virtual void SetDamage(int damage)
	{
		Damage = damage;
	}

	public virtual void SetRadius(float radius)
	{
		Radius = radius;
	}

	protected abstract bool OnUse(Lives livesComponent);
}