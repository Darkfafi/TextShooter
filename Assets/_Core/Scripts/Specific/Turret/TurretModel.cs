using System;
using UnityEngine;

public class TurretModel : EntityModel, ITargetingUser
{
	public delegate void NewOldTargetHandler(EntityModel newTarget, EntityModel previousTarget);
	public event Action<BaseWeapon> WeaponChangedEvent;
	public event Action<float> RadiusChangedEvent;
	public event Action<bool> GunActiveStateChangedEvent;
	public event Action GunFiredEvent;

	public EntityModel CurrentTarget
	{
		get
		{
			if(Targeting == null)
				return null;

			return Targeting.TargetsFilter.GetFirst(
				(e) => 
				{
					if(!Targeting.IsTargetCompleted(e))
						return false;

					return true;
				}, this.SortOnClosestTo());
		}
	}

	public Lives Lives
	{
		get; private set;
	}

	public Targeting Targeting
	{
		get; private set;
	}

	public float TurretNeckRotation
	{
		get; private set;
	}

	public float Radius
	{
		get
		{
			return (_weaponHolder == null || !_weaponHolder.HasWeapon) ? 0f : _weaponHolder.Weapon.Radius;
		}
	}

	public bool IsGunActive
	{
		get
		{
			return _weaponHolder != null && _weaponHolder.HasWeapon && _weaponHolder.IsEnabled;
		}
	}

	public bool AddTargetingUserTagOnCreation
	{
		get
		{
			return true;
		}
	}

	public Vector3 TargetingUserPosition
	{
		get
		{
			return ModelTransform.Position;
		}
	}

	private TimekeeperModel _timekeeper;
	private float _rotationTillResultTile = 0f;
	private WeaponHolder _weaponHolder;

	public TurretModel(TimekeeperModel timekeeper)
	{
		_timekeeper = timekeeper;
		_timekeeper.ListenToFrameTick(Update);
		_weaponHolder = AddComponent<WeaponHolder>();

		ModelTags.AddTag(Tags.ENEMY_TARGET);

		Lives = AddComponent<Lives>();
		Lives.SetLivesAmount(3);
		Lives.DeathEvent += OnDeathEvent;

		SetWeapon(new InstantHitGun(0.25f, timekeeper, 10f));
		SetGunActiveState(true);
	}

	protected override void OnModelDestroy()
	{
		base.OnModelDestroy();

		_timekeeper.UnlistenFromFrameTick(Update);
		_timekeeper = null;

		_weaponHolder = null;

		Lives.DeathEvent -= OnDeathEvent;
		Lives = null;
		Targeting = null;
	}

	private void OnDeathEvent(Lives livesComponent)
	{
		Destroy();
	}

	private void Update(float deltaTime, float timeScale)
	{
		float angleToTarget = 0;

		if(CurrentTarget != null && !CurrentTarget.IsDestroyed && IsGunActive)
		{
			if(Vector2.Distance(ModelTransform.Position, CurrentTarget.ModelTransform.Position) < Radius)
			{
				float x = CurrentTarget.ModelTransform.Position.x - ModelTransform.Position.x;
				float y = CurrentTarget.ModelTransform.Position.y - ModelTransform.Position.y;

				angleToTarget = (Mathf.Atan2(y, x) * Mathf.Rad2Deg) - 90f;
				_rotationTillResultTile += deltaTime * timeScale;
				if(Mathf.Abs(Mathf.DeltaAngle(angleToTarget, TurretNeckRotation)) < 10f)
				{
					_rotationTillResultTile = 0f;
					Lives currentTargetLives = CurrentTarget.GetComponent<Lives>();
					if(_weaponHolder.UseWeaponIfAny(currentTargetLives))
					{
						if(GunFiredEvent != null)
						{
							GunFiredEvent();
						}
					}
				}
			}
		}

		TurretNeckRotation = Mathf.LerpAngle(TurretNeckRotation, angleToTarget, deltaTime * timeScale * 7.4f * (1f + _rotationTillResultTile));
	}

	// Setters and Getters

	public void SetWeapon(BaseWeapon weapon)
	{
		if(_weaponHolder.Weapon != weapon)
		{
			_weaponHolder.SetWeapon(weapon);

			if(WeaponChangedEvent != null)
			{
				WeaponChangedEvent(weapon);
			}
		}
	}

	public void SetGunActiveState(bool activeState)
	{
		if(_weaponHolder != null && activeState != IsGunActive)
		{
			_weaponHolder.SetEnabledState(activeState);

			if(GunActiveStateChangedEvent != null)
			{
				GunActiveStateChangedEvent(IsGunActive);
			}
		}
	}

	public void SetRadius(float newRadius)
	{
		if(!_weaponHolder.HasWeapon)
			return;

		float preRange = Radius;
		_weaponHolder.Weapon.SetRadius(newRadius);
		if(preRange != Radius)
		{
			if(RadiusChangedEvent != null)
			{
				RadiusChangedEvent(Radius);
			}
		}
	}

	public void SetCurrentTargeting(Targeting targeting)
	{
		Targeting = targeting;
	}
}
