﻿using System;

public class EnemyModel : EntityModel
{
    public enum EnemyCharacterType
    {
        Normal, // Normal word behaviour
        Double, // Each word is to be typed 2 times
    }

    public WordsHolder WordsHolder { get; private set; }
    public WordsHitter WordsHitter { get; private set; }
    public int Damage { get; private set; }
    public EnemyCharacterType EnemyType { get; private set; }

    public bool IsDead
    {
        get
        {
            return WordsHolder.WordsAmount(true) == 0;
        }
    }

    public EnemyModel(EnemyCharacterType enemyType, int damage, string currentWord, params string[] nextWords)
    {
        EnemyType = enemyType;
        Damage = damage;
        WordsHolder = new WordsHolder(currentWord, nextWords);
        WordsHitter = new WordsHitter(WordsHolder, GetCharHitsNeeded());
    }

    protected override void OnEntityDestroy()
    {
        WordsHitter.Clean();
        WordsHolder.Clean();

        WordsHitter = null;
        WordsHolder = null;
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
