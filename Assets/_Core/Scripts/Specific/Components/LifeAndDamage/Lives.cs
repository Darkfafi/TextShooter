using System;
using UnityEngine;

public class Lives : BaseModelComponent
{
	public const int DAMAGE_KILL = -1337;

	public event Action<Lives> DeathEvent;
	public event Action<Lives, int> DamageEvent;

	public int LivesAmount
	{
		get
		{
			return _lives;
		}
		private set
		{
			_lives = value;
		}
	}

	public bool IsAlive
	{
		get
		{
			return LivesAmount > 0;
		}
	}

	[ModelEditorField]
	private int _lives = 1;

	[ModelEditorMethod]
	public void Damage(int damageAmount)
	{
		if(damageAmount < 0)
		{
			if(damageAmount == DAMAGE_KILL)
				Kill();

			return;
		}

		LivesAmount = Mathf.Max(LivesAmount - damageAmount, 0);

		if(DamageEvent != null)
			DamageEvent(this, damageAmount);

		if(LivesAmount == 0)
		{
			if(DeathEvent != null)
			{
				DeathEvent(this);
			}
		}
	}

	[ModelEditorMethod]
	public void SetLivesAmount(int amount)
	{
		LivesAmount = amount;
	}

	[ModelEditorMethod]
	public void Kill()
	{
		Damage(LivesAmount);
	}
}
