public class InstantHitGun : BaseCooldownWeapon
{
	public InstantHitGun(float cooldown, TimekeeperModel timekeeper, float radius, int damage = 1) : base(cooldown, timekeeper, radius, damage)
	{

	}

	protected override bool DoUseLogics(Lives livesComponent)
	{
		livesComponent.Damage(Damage);
		return true;
	}
}
