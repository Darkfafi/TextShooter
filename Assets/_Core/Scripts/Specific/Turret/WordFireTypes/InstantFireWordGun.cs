public class InstantHitGun : BaseCooldownWeapon
{
	protected override bool DoUseLogics(Lives livesComponent)
	{
		ApplyDamage(livesComponent);
		return true;
	}
}
