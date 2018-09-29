public class WordsHitter
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

    public int TargetWordCharIndex { get; private set; }
    public int HitsOnCharDone { get; private set; }

    public WordsHitter(WordsHolder wordsHolder, int charHitsNeeded)
    {
        _wordsHolder = wordsHolder;
        _charHitsNeeded = charHitsNeeded;
    }

    public void Hit(char hitChar)
    {
        if(string.IsNullOrEmpty(_wordsHolder.CurrentWord))
        {
            return;
        }

        if (_wordsHolder.CurrentWord[TargetWordCharIndex] == hitChar)
        {
            WordHitInternal(_charHitsNeeded);
        }
    }

    public void Clean()
    {
        _wordsHolder = null;
    }

    private void WordHitInternal(int hitsNeeded)
    {
        string wordToHit = _wordsHolder.CurrentWord;
        int charIndexToHit = TargetWordCharIndex;
        HitsOnCharDone++;

        if (HitsOnCharDone >= hitsNeeded)
        {
            if (charIndexToHit >= wordToHit.Length - 1)
            {
                if (_wordsHolder.CycleToNextWord())
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

        if (WordCharHitEvent != null)
        {
            WordCharHitEvent(wordToHit, charIndexToHit, this);
        }
    }
}
