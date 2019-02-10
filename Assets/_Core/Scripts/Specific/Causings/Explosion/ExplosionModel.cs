using System;
using UnityEngine;

public class ExplosionModel : EntityModel
{
	public const int EXPLOSION_DESTROY_PERMISSION = 0;

	public static void DoExplosion(Vector2 position, int damageToInflict, params Lives[] livesToHit)
	{
		ExplosionModel explosion = new ExplosionModel(position);
		explosion.Explode(damageToInflict, livesToHit);
	}

	private ExplosionModel(Vector2 position) : base(position)
	{

	}

	private void Explode(int damageToInflict, params Lives[] livesToHit)
	{
		for(int i = 0; i < livesToHit.Length; i++)
		{
			if(livesToHit[i].ComponentState == ModelComponentState.Active)
				livesToHit[i].Damage(damageToInflict);
		}

		MethodPermitter.ExecuteWhenPermitted(EXPLOSION_DESTROY_PERMISSION, Destroy);
	}
}
