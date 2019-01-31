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
			return _weapon.Radius;
		}
	}

	public bool IsGunActive
	{
		get
		{
			return _weapon != null && _weapon.IsEnabled;
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
	private BaseWeapon _weapon;

	public TurretModel(TimekeeperModel timekeeper)
	{
		_timekeeper = timekeeper;
		_timekeeper.ListenToFrameTick(Update);

		InstantHitGun defaultGun = SetWeapon<InstantHitGun>();
		defaultGun.SetupCooldown(0.25f, timekeeper);
		defaultGun.SetRadius(8f);

		SetGunActiveState(true);

		ModelTags.AddTag(Tags.ENEMY_TARGET);

		Lives = AddComponent<Lives>();
		Lives.SetLivesAmount(3);
		Lives.DeathEvent += OnDeathEvent;
	}

	protected override void OnModelDestroy()
	{
		base.OnModelDestroy();

		_timekeeper.UnlistenFromFrameTick(Update);
		_timekeeper = null;

		_weapon = null;

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
					if(_weapon.Use(currentTargetLives))
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

	public T SetWeapon<T>() where T : BaseWeapon
	{
		if(_weapon != null)
		{
			RemoveComponent(_weapon);
		}

		_weapon = AddComponent<T>();

		if(WeaponChangedEvent != null)
		{
			WeaponChangedEvent(_weapon);
		}

		return _weapon as T;
	}

	public void SetGunActiveState(bool activeState)
	{
		if(_weapon != null && activeState != IsGunActive)
		{
			_weapon.SetEnabledState(activeState);

			if(GunActiveStateChangedEvent != null)
			{
				GunActiveStateChangedEvent(IsGunActive);
			}
		}
	}

	public void SetRadius(float newRadius)
	{
		float preRange = Radius;
		_weapon.SetRadius(newRadius);
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
