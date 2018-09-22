using System;

public class EnemyModel : EntityModel
{
    public enum EnemyCharacterType
    {
        Normal, // Normal word behaviour
        Double, // Each word is to be typed 2 times
    }

    public event Action<EnemyModel> CurrentWordSetEvent;
    public event Action<EnemyModel> DeathEvent;

    public WordsHolder WordsHolder { get; private set; }
    public int Damage { get; private set; }
    public EnemyCharacterType EnemyType { get; private set; }

    public bool IsDead
    {
        get
        {
            return WordsHolder.WordsAmount(true) == 0;
        }
    }

    private int _currentWordChar = 0;
    private int _hitsOnChar = 0;

    public EnemyModel(EnemyCharacterType enemyType, int damage, string currentWord, params string[] nextWords)
    {
        WordsHolder = new WordsHolder(currentWord, nextWords);
        EnemyType = enemyType;
        Damage = damage;
    }

    public void Hit(char hitChar)
    {
        if(WordsHolder.CurrentWord[_currentWordChar] == hitChar)
        {
            _hitsOnChar++;
            switch(EnemyType)
            {
                case EnemyCharacterType.Double:
                    WordHitInternal(2);
                    break;

                default:
                    WordHitInternal(1);
                    break;
            }
        }
    }

    private void WordHitInternal(int hitsNeeded)
    {
        if (_hitsOnChar >= hitsNeeded)
        {
            if(_currentWordChar >= WordsHolder.CurrentWord.Length - 1)
            {
                if(WordsHolder.CycleToNextWord())
                {
                    // Next Word
                    _currentWordChar = 0;
                    _hitsOnChar = 0;

                    if (CurrentWordSetEvent != null)
                    {
                        CurrentWordSetEvent(this);
                    }
                }
                else
                {
                    // No words == Death
                    if(DeathEvent != null)
                    {
                        DeathEvent(this);
                    }
                }
            }
            else
            {
                // Next Character in word
                _currentWordChar++;
                _hitsOnChar = 0;
                if (CurrentWordSetEvent != null)
                {
                    CurrentWordSetEvent(this);
                }
            }
        }
    }
}
