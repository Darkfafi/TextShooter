using System;
using UnityEngine;

public class TurretModel : EntityModel, ITargetingUser
{
	public delegate void NewOldTargetHandler(EntityModel newTarget, EntityModel previousTarget);
	public event Action<IFireWordGun> FireWordGunChangedEvent;
	public event Action<float> RangeChangedEvent;
	public event Action<float> CooldownChangedEvent;
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

	public float Range
	{
		get
		{
			return _fireWordGun.Range;
		}
	}

	public float Cooldown
	{
		get
		{
			return _fireWordGun.Cooldown;
		}
	}

	public bool IsGunActive
	{
		get; private set;
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
	private BaseFireWordGun _fireWordGun;

	public TurretModel(TimekeeperModel timekeeper)
	{
		_timekeeper = timekeeper;
		_timekeeper.ListenToFrameTick(Update);

		SetFireWordGun(new InstantFireWordGun(0.25f, 8f, timekeeper));
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

		_fireWordGun = null;

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
			if(Vector2.Distance(ModelTransform.Position, CurrentTarget.ModelTransform.Position) < Range)
			{
				float x = CurrentTarget.ModelTransform.Position.x - ModelTransform.Position.x;
				float y = CurrentTarget.ModelTransform.Position.y - ModelTransform.Position.y;

				angleToTarget = (Mathf.Atan2(y, x) * Mathf.Rad2Deg) - 90f;
				_rotationTillResultTile += deltaTime * timeScale;
				if(Mathf.Abs(Mathf.DeltaAngle(angleToTarget, TurretNeckRotation)) < 10f)
				{
					_rotationTillResultTile = 0f;
					WordsHp currentTargetWordsHp = CurrentTarget.GetComponent<WordsHp>();
					if(_fireWordGun.Fire(currentTargetWordsHp, currentTargetWordsHp.CurrentTargetWord))
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

	public void SetFireWordGun(BaseFireWordGun fireWordGun)
	{
		if(fireWordGun != null)
		{
			_fireWordGun = fireWordGun;
			if(FireWordGunChangedEvent != null)
			{
				FireWordGunChangedEvent(_fireWordGun);
			}
		}
	}

	public void SetGunActiveState(bool activeState)
	{
		if(IsGunActive == activeState)
			return;

		IsGunActive = activeState;

		if(GunActiveStateChangedEvent != null)
		{
			GunActiveStateChangedEvent(IsGunActive);
		}
	}

	public void SetRange(float newRange)
	{
		float preRange = Range;
		_fireWordGun.SetRange(newRange);
		if(preRange != Range)
		{
			if(RangeChangedEvent != null)
			{
				RangeChangedEvent(Range);
			}
		}
	}

	public void SetCooldown(float cooldown)
	{
		float preCooldown = Cooldown;
		_fireWordGun.SetCooldown(cooldown);
		if(preCooldown != Cooldown)
		{
			if(CooldownChangedEvent != null)
			{
				CooldownChangedEvent(Cooldown);
			}
		}
	}

	public void SetCurrentTargeting(Targeting targeting)
	{
		Targeting = targeting;
	}
}
