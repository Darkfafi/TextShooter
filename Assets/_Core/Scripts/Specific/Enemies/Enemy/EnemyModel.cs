using System;
using UnityEngine;

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

	public EnemyModel(TimekeeperModel timekeeper, Vector3 position) : base(position)
	{
		AddComponent<TopDownMovement>().Setup(timekeeper);
		ModelTags.AddTag(Tags.DISPLAY_WORD);
	}

	public void Initialize(string currentWord, params string[] nextWords)
	{
		if(WordsHolder != null)
			return;

		WordsHolder = AddComponent<WordsHolder>();
		WordsHolder.Setup(currentWord, nextWords);
		WordsHp = AddComponent<WordsHp>();

		WordsHolder.WordCycledEvent += OnWordCycledEvent;
	}

	protected override void OnModelDestroy()
	{
		base.OnModelDestroy();

		WordsHolder.WordCycledEvent -= OnWordCycledEvent;

		WordsHolder = null;
		WordsHp = null;
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
