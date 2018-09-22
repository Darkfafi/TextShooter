using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyModel : EntityModel
{
    public WordsHolder WordsHolder { get; private set; }

    public EnemyModel(string currentWord, params string[] nextWords)
    {
        WordsHolder = new WordsHolder(currentWord, nextWords);
    }
}
