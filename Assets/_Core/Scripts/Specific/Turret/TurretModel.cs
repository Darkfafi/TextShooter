using System;
using UnityEngine;

public class TurretModel : EntityModel
{
	public delegate void NewOldTargetHandler(EntityModel newTarget, EntityModel previousTarget);
	public event NewOldTargetHandler TargetSetEvent;
	public event Action<IFireWordGun> FireWordGunChangedEvent;
	public event Action<float> RangeChangedEvent;
	public event Action<float> CooldownChangedEvent;
	public event Action<bool> GunActiveStateChangedEvent;
	public event Action GunFiredEvent;

	public EntityModel CurrentTarget
	{
		get
		{
			if(TargetSystem == null)
				return null;

			return TargetSystem.TargetsFilter.GetFirst(
				(e) => 
				{
					if(!TargetSystem.IsTargetCompleted(e))
						return false;

					return true;
				}, (a, b) =>
				{
					float distA = (a.ModelTransform.Position - ModelTransform.Position).magnitude;
					float distB = (b.ModelTransform.Position - ModelTransform.Position).magnitude;
					return (int)(distA - distB);
				});
		}
	}

	public TargetSystem TargetSystem
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

	private TimekeeperModel _timekeeper;

	private BaseFireWordGun _fireWordGun;

	public TurretModel(TimekeeperModel timekeeper, CharInputModel charInputModel)
	{
		_timekeeper = timekeeper;
		_timekeeper.ListenToFrameTick(Update);

		TargetSystem = AddComponent<TargetSystem>();
		TargetSystem.SetupTargetSystem(charInputModel, FilterRules.CreateHasAnyTagsFilter(Tags.ENEMY));

		ModelTags.AddTag(Tags.DISPLAY_TARGETING);

		SetFireWordGun(new InstantFireWordGun(0.25f, 5f, timekeeper));
		SetGunActiveState(true);
	}

	protected override void OnModelDestroy()
	{
		base.OnModelDestroy();

		_timekeeper.UnlistenFromFrameTick(Update);
		_timekeeper = null;

		_fireWordGun = null;

		TargetSystem = null;
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

				if(Mathf.Abs(Mathf.DeltaAngle(angleToTarget, TurretNeckRotation)) < 10f)
				{
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

		TurretNeckRotation = Mathf.LerpAngle(TurretNeckRotation, angleToTarget, deltaTime * timeScale * 7.4f);
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
}
