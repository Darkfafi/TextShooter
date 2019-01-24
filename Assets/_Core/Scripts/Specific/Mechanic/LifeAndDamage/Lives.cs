using System;
using UnityEngine;

public class Lives : BaseModelComponent
{
	public event Action<Lives> DeathEvent;

	public int Amount
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
			return Amount > 0;
		}
	}

	[ModelEditorField]
	private int _lives = 1;

	[ModelEditorMethod]
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

	[ModelEditorMethod]
	public void SetLivesAmount(int amount)
	{
		Amount = amount;
	}

	[ModelEditorMethod]
	public void Kill()
	{
		Damage(Amount);
	}
}
