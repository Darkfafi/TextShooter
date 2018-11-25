public class WordsHitter : BaseModelComponent
{
	public delegate void WordCharHandler(string word, int charIndex, WordsHitter hitter);
	public event WordCharHandler WordCharHitEvent;

	private WordsHolder _wordsHolder;
	private int _charHitsNeeded;

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
	public int HitsOnCharDone
	{
		get; private set;
	}

	public void SetCharHitsNeeded(int charHitsNeeded)
	{
		_charHitsNeeded = charHitsNeeded;
	}

	protected override void Added()
	{
		_wordsHolder = Components.GetComponent<WordsHolder>();
	}

	protected override void Removed()
	{
		_wordsHolder = null;
	}

	public void Hit(char hitChar)
	{
		if(string.IsNullOrEmpty(_wordsHolder.CurrentWord))
		{
			return;
		}

		if(_wordsHolder.CurrentWord[TargetWordCharIndex] == hitChar)
		{
			WordHitInternal(_charHitsNeeded);
		}
	}

	private void WordHitInternal(int hitsNeeded)
	{
		string wordToHit = _wordsHolder.CurrentWord;
		int charIndexToHit = TargetWordCharIndex;
		HitsOnCharDone++;

		if(HitsOnCharDone >= hitsNeeded)
		{
			if(charIndexToHit >= wordToHit.Length - 1)
			{
				if(_wordsHolder.CycleToNextWord())
				{
					// Next Word
					TargetWordCharIndex = 0;
					HitsOnCharDone = 0;
				}
			}
			else
			{
				// Next Character in word
				TargetWordCharIndex++;
				HitsOnCharDone = 0;
			}
		}

		if(WordCharHitEvent != null)
		{
			WordCharHitEvent(wordToHit, charIndexToHit, this);
		}
	}
}
