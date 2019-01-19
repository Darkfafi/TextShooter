public abstract class BaseWeapon : BaseModelComponent
{
	public static int DAMAGE_KILL = -1;

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

	public BaseWeapon()
	{
		Damage = 1;
		Radius = 1f;
	}

	public void Use(Lives livesComponent)
	{
		if(CanBeUsed)
		{
			OnUse(livesComponent);
		}
	}

	public virtual void SetDamage(int damage)
	{
		Damage = damage;
	}

	public virtual void SetRadius(float radius)
	{
		Radius = radius;
	}

	protected void ApplyDamage(Lives livesComponent)
	{
		if(Damage == DAMAGE_KILL)
		{
			livesComponent.Kill();
		}
		else
		{
			livesComponent.Damage(Damage);
		}
	}

	protected abstract void OnUse(Lives livesComponent);
}