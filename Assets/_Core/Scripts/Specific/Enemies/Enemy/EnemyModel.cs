using System;

public class EnemyModel : EntityModel
{
    public enum EnemyCharacterType
    {
        Normal, // Normal word behaviour
        Double, // Each word is to be typed 2 times
    }

    public event Action<EnemyModel> DeathEvent;

    public WordsHolder WordsHolder { get; private set; }
    public WordsHitter WordsHitter { get; private set; }

    public AIModel AIModel { get; private set; }
    public int Damage { get; private set; }
    public EnemyCharacterType EnemyType { get; private set; }

    public bool IsDead
    {
        get
        {
            return WordsHolder == null || WordsHolder.WordsAmount(true) == 0;
        }
    }

    public EnemyModel(TimekeeperModel timekeeper, EnemyCharacterType enemyType, string currentWord, params string[] nextWords)
    {
        EnemyType = enemyType;
        Damage = 1;

        AddComponent<TopDownMovement>().Setup(timekeeper);
        WordsHolder = AddComponent<WordsHolder>();
        WordsHolder.Setup(currentWord, nextWords);
        AddComponent<WordsHitter>().SetCharHitsNeeded(GetCharHitsNeeded());

        AIModel = new AIModel();

        WordsHolder.WordCycledEvent += OnWordCycledEvent;

        ModelTags.AddTag("Enemy");
    }

    public void SetDamage(int damage)
    {
        Damage = damage;
    }

    protected override void OnModelDestroy()
    {
        base.OnModelDestroy();

        WordsHolder.WordCycledEvent -= OnWordCycledEvent;

        WordsHitter = null;
        WordsHolder = null;
    }

    private void OnWordCycledEvent(string previousWord, string newWord)
    {
        if(string.IsNullOrEmpty(newWord))
        {
            if(DeathEvent != null)
            {
                DeathEvent(this);
            }
        }
    }

    private int GetCharHitsNeeded()
    {
        switch (EnemyType)
        {
            case EnemyCharacterType.Double:
                return 2;
            default:
                return 1;
        }
    }
}
