using UnityEngine;

public class WordsLive : BaseModelComponent
{
	public WordsHolder WordsHolder
	{
		get
		{
			if(_wordsHolder == null)
				return GetComponent<WordsHolder>();

			return _wordsHolder;
		}
		private set
		{
			_wordsHolder = value;
		}
	}

	public Lives Lives
	{
		get
		{
			if(_lives == null)
				return GetComponent<Lives>();

			return _lives;
		}
		private set
		{
			_lives = value;
		}
	}

	private bool _addedLives = false;
	private bool _addedWordsHolder = false;

	private Lives _lives;
	private WordsHolder _wordsHolder;

	private bool _setup = false;

	public void Setup(string currentWord, params string[] nextWords)
	{
		if(_setup)
			return;

		_setup = true;

		WordsHolder.Setup(currentWord, nextWords);
		Lives.SetLivesAmount(WordsHolder.GetWordsAmount());
		Lives.DamageEvent += OnDamageEvent;
		WordsHolder.WordCycledEvent += OnWordCycledEvent;
	}

	protected override void Added()
	{
		_addedLives = RequireComponent(out _lives);
		_addedWordsHolder = RequireComponent(out _wordsHolder);
	}

	protected override void Removed()
	{
		WordsHolder.WordCycledEvent -= OnWordCycledEvent;

		Lives.DamageEvent -= OnDamageEvent;

		if(_addedLives)
			RemoveComponent(Lives);

		Lives = null;

		if(_addedWordsHolder)
			RemoveComponent(WordsHolder);

		WordsHolder = null;
	}

	private void OnDamageEvent(Lives livesComponent, int damage)
	{
		int wordAmountAffected = Mathf.Clamp(damage, 0, WordsHolder.GetWordsAmount());
		for(int i = 0; i < wordAmountAffected; i++)
		{
			WordsHolder.CycleToNextWord();
		}
	}

	private void OnWordCycledEvent(string previousWord, string newWord, WordsHolder wordsHolder)
	{
		Lives.SetLivesAmount(WordsHolder.GetWordsAmount());
		if(string.IsNullOrEmpty(newWord))
		{
			Lives.Kill(); // No words left, so kill enemy
		}
	}
}
