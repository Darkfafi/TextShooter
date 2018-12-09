public class WordsHp : BaseModelComponent
{
	public delegate void WordCharHandler(string word, int charIndex, WordsHp hitter);
	public event WordCharHandler WordCharHitEvent;

	private WordsHolder _wordsHolder;

	public string CurrentTargetWord
	{
		get
		{
			return _wordsHolder.CurrentWord;
		}
	}

	public int TargetWordCharIndex
	{
		get; private set;
	}

	public bool IsDead
	{
		get
		{
			return string.IsNullOrEmpty(_wordsHolder.CurrentWord);
		}
	}

	protected override void Added()
	{
		_wordsHolder = GetComponent<WordsHolder>();
	}

	protected override void Removed()
	{
		_wordsHolder = null;
	}

	public static bool IsHit(char charForHit, char charToHit)
	{
		return charForHit.ToString().ToLower() == charToHit.ToString().ToLower();
	}

	public bool Hit(char hitChar)
	{
		if(IsDead)
		{
			return false;
		}

		if(IsHit(hitChar, CurrentTargetWord[TargetWordCharIndex]))
		{
			WordHitInternal();
			return true;
		}

		return false;
	}

	public bool HitEntireWord()
	{
		if(IsDead)
		{
			return false;
		}

		string word = CurrentTargetWord;

		for(int i = 0, c = word.Length; i < c; i++)
		{
			Hit(word[i]);
		}

		return true;
	}

	public char GetCurrentChar()
	{
		return GetChar(TargetWordCharIndex);
	}

	public char GetChar(int index)
	{
		if(string.IsNullOrEmpty(CurrentTargetWord) || index < 0 || index >= CurrentTargetWord.Length)
		{
			return default(char);
		}

		return CurrentTargetWord[index];
	}

	private void WordHitInternal()
	{
		string wordToHit = _wordsHolder.CurrentWord;
		int charIndexToHit = TargetWordCharIndex;

		if(charIndexToHit >= wordToHit.Length - 1)
		{
			if(_wordsHolder.CycleToNextWord())
			{
				// Next Word
				TargetWordCharIndex = 0;
			}
		}
		else
		{
			// Next Character in word
			TargetWordCharIndex++;
		}
		

		if(WordCharHitEvent != null)
		{
			WordCharHitEvent(wordToHit, charIndexToHit, this);
		}
	}
}
