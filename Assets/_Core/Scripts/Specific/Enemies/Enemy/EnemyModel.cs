using System;

public class EnemyModel : EntityModel
{
	public event Action<EnemyModel> DeathEvent;

	public WordsHolder WordsHolder
	{
		get; private set;
	}

	public WordsHp WordsHp
	{
		get; private set;
	}

	public bool IsDead
	{
		get
		{
			return WordsHolder == null || WordsHolder.WordsAmount(true) == 0;
		}
	}

	public EnemyModel(TimekeeperModel timekeeper, string currentWord, params string[] nextWords)
	{
		AddComponent<TopDownMovement>().Setup(timekeeper);
		WordsHolder = AddComponent<WordsHolder>();
		WordsHolder.Setup(currentWord, nextWords);
		WordsHp = AddComponent<WordsHp>();

		WordsHolder.WordCycledEvent += OnWordCycledEvent;

		ModelTags.AddTag(Tags.ENEMY);
		ModelTags.AddTag(Tags.DISPLAY_WORD);
	}

	protected override void OnModelDestroy()
	{
		base.OnModelDestroy();

		WordsHolder.WordCycledEvent -= OnWordCycledEvent;

		WordsHp = null;
		WordsHolder = null;
	}

	private void OnWordCycledEvent(string previousWord, string newWord, WordsHolder wordsHolder)
	{
		if(string.IsNullOrEmpty(newWord))
		{
			if(DeathEvent != null)
			{
				DeathEvent(this);
			}

			Destroy();
		}
	}
}
