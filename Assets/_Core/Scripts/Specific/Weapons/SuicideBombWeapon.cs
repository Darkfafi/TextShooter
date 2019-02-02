public class SuicideBombWeapon : BaseWeapon
{
	public const string ID_WEAPON = "suicide";

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
		ExplosionModel.DoExplosion(Holder.GetComponent<ModelTransform>().Position, Damage, livesComponent);

		if(Holder.HasComponent<Lives>())
			Holder.GetComponent<Lives>().Kill();

		return true;
	}
}
