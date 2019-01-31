using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseCooldownWeapon : BaseWeapon
{
	public override bool CanBeUsed
	{
		get
		{
			return _cooldownProcess >= Cooldown;
		}
	}

	public float Cooldown
	{
		get; private set;
	}

	private float _cooldownProcess;
	private TimekeeperModel _timekeeper;

	public void SetCooldown(float newValue)
	{
		Cooldown = newValue;
	}

	public void SetupCooldown(float cooldown, TimekeeperModel timekeeper)
	{
		if(_timekeeper != null)
			return;

		_timekeeper = timekeeper;
		SetCooldown(cooldown);
		_timekeeper.ListenToFrameTick(OnUpdate);
	}

	protected override bool OnUse(Lives livesComponent)
	{
		_cooldownProcess = 0f;
		return DoUseLogics(livesComponent);
	}

	protected override void Removed()
	{
		if(_timekeeper != null)
		{
			_timekeeper.UnlistenFromFrameTick(OnUpdate);
			_timekeeper = null;
		}
	}

	protected abstract bool DoUseLogics(Lives livesComponent);

	private void OnUpdate(float deltaTime, float timeScale)
	{
		if(Cooldown > _cooldownProcess)
		{
			_cooldownProcess += deltaTime * timeScale;
		}
	}
}
