public class SuicideBombWeapon : BaseWeapon
{
	public override bool CanBeUsed
	{
		get
		{
			Lives l = Holder.GetComponent<Lives>();
			return l == null || l.IsAlive;
		}
	}

	public SuicideBombWeapon(float radius, int damage = 1) : base(radius, damage)
	{

	}

	protected override bool OnUse(Lives livesComponent)
	{
		if(Holder.HasComponent<Lives>())
			Holder.GetComponent<Lives>().Kill();

		livesComponent.Damage(Damage);

		return true;
	}
}
