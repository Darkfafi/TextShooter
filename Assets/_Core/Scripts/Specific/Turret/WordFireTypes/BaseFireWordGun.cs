using System;
using System.Text;

public abstract class BaseFireWordGun : IFireWordGun
{
	public delegate void WordHpWordHandler(WordsHp hpFiredAt, string stringFired);
	public WordHpWordHandler FiredEvent;
	public Action<float> RangeChangedEvent;
	public Action<float> CooldownChangedEvent;

	public float Range
	{
		get; private set;
	}

	public float Cooldown
	{
		get; private set;
	}

	private float _cooldownProcess;
	private TimekeeperModel _timekeeper;

	public BaseFireWordGun(float cooldown, float range, TimekeeperModel timekeeper)
	{
		_timekeeper = timekeeper;
		_timekeeper.ListenToFrameTick(OnUpdate);
		SetRange(range);
		SetCooldown(cooldown);
	}

	public bool Fire(WordsHp wordsHp, string wordToFireAsChars)
	{
		if(_cooldownProcess >= Cooldown)
		{
			if(FireLogics(wordsHp, wordToFireAsChars))
			{
				_cooldownProcess = 0f;
				if(FiredEvent != null)
				{
					FiredEvent(wordsHp, wordToFireAsChars);
				}

				return true;
			}
		}

		return false;
	}

	public void SetRange(float range)
	{
		if(Range != range)
		{
			Range = range;
			if(RangeChangedEvent != null)
			{
				RangeChangedEvent(Range);
			}
		}
	}

	public void SetCooldown(float cooldown)
	{
		if(Cooldown != cooldown)
		{
			Cooldown = cooldown;
			if(CooldownChangedEvent != null)
			{
				CooldownChangedEvent(Cooldown);
			}
		}
	}

	protected abstract bool FireLogics(WordsHp wordsHp, string wordToFireAsChars);

	public bool Fire(WordsHp wordsHp, char[] charsToFire)
	{
		StringBuilder sb = new StringBuilder(charsToFire.Length);
		sb.Append(charsToFire);
		return Fire(wordsHp, sb.ToString());
	}

	public void Clean()
	{
		_timekeeper.UnlistenFromFrameTick(OnUpdate);
		_timekeeper = null;
	}

	private void OnUpdate(float deltaTime, float timeScale)
	{
		if(Cooldown > _cooldownProcess)
		{
			_cooldownProcess += deltaTime * timeScale;
		}
	}
}

public interface IFireWordGun
{
	bool Fire(WordsHp wordsHp, char[] charsToFire);
	bool Fire(WordsHp wordsHp, string wordToFireAsChars);
}
