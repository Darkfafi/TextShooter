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
	public BaseCooldownWeapon(float cooldown, TimekeeperModel timekeeper, float radius, int damage = 1) : base(radius, damage)
	{
		_timekeeper = timekeeper;
		SetCooldown(cooldown);
		_timekeeper.ListenToFrameTick(OnUpdate);
	}

	public void SetCooldown(float newValue)
	{
		Cooldown = newValue;
	}

	protected override bool OnUse(Lives livesComponent)
	{
		_cooldownProcess = 0f;
		return DoUseLogics(livesComponent);
	}

	public override void Clean()
	{
		if(_timekeeper != null)
		{
			_timekeeper.UnlistenFromFrameTick(OnUpdate);
			_timekeeper = null;
		}
		base.Clean();
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
