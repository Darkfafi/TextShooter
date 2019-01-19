using System;
using UnityEngine;

public class Lives : BaseModelComponent
{
	public event Action<Lives> DeathEvent;

	public int Amount
	{
		get; private set;
	}

	public bool IsAlive
	{
		get
		{
			return Amount > 0;
		}
	}

	protected override void Added()
	{
		Amount = 1;
	}

	public void Damage(int amount)
	{
		if(amount < 0)
			return;

		Amount = Mathf.Max(Amount - amount, 0);
		if(Amount == 0)
		{
			if(DeathEvent != null)
			{
				DeathEvent(this);
			}
		}
	}

	public void SetLivesAmount(int amount)
	{
		Amount = amount;
	}

	public void Kill()
	{
		Damage(Amount);
	}
}
