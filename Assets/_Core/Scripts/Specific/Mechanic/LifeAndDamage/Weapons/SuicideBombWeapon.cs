public class SuicideBombWeapon : BaseWeapon
{
	public override bool CanBeUsed
	{
		get
		{
			return true;
		}
	}

	public SuicideBombWeapon()
	{
		SetDamage(1);
		SetRadius(2f);
	}

	protected override void OnUse(Lives livesComponent)
	{
		if(Parent.HasComponent<Lives>())
			Parent.GetComponent<Lives>().Kill();

		ApplyDamage(livesComponent);
	}
}
