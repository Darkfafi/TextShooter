using UnityEngine;

public class EnemyModel : EntityModel, IStateMachineAffected
{
	public WordsHolder WordsHolder
	{
		get; private set;
	}

	public WordsHp WordsHp
	{
		get; private set;
	}

	public Lives Lives
	{
		get; private set;
	}

	public bool IsDead
	{
		get
		{
			if(Lives == null || Lives.ComponentState == ModelComponentState.Removed)
				return true;

			return !Lives.IsAlive;
		}
	}

	public EnemyModel(TimekeeperModel timekeeper, Vector3 position) : base(position)
	{
		AddComponent<TopDownMovement>().Setup(timekeeper);
		ModelTags.AddTag(Tags.DISPLAY_WORD);
	}

	public void Initialize(string currentWord, params string[] nextWords)
	{
		if(WordsHolder != null)
			return;

		Lives = AddComponent<Lives>();
		WordsHolder = AddComponent<WordsHolder>();
		WordsHolder.Setup(currentWord, nextWords);
		WordsHp = AddComponent<WordsHp>();

		Lives.DeathEvent += OnDeathEvent;
		WordsHolder.WordCycledEvent += OnWordCycledEvent;
	}

	protected override void OnModelDestroy()
	{
		base.OnModelDestroy();

		Lives.DeathEvent -= OnDeathEvent;
		WordsHolder.WordCycledEvent -= OnWordCycledEvent;

		Lives = null;
		WordsHolder = null;
		WordsHp = null;
	}

	private void OnDeathEvent(Lives livesComponent)
	{
		Destroy();
	}

	private void OnWordCycledEvent(string previousWord, string newWord, WordsHolder wordsHolder)
	{
		if(string.IsNullOrEmpty(newWord))
		{
			Lives.Kill(); // No words left, so kill enemy
		}
	}
}
