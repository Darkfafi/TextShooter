using System.Collections.Generic;

public class WordsHolder
{
    public delegate void PreNewWordHandler(string previousWord, string newWord);
    public event PreNewWordHandler WordCycledEvent;

    public string CurrentWord { get; private set; }
    private List<string> _words = new List<string>();

    public WordsHolder(string startWord, params string[] nextWords)
    {
        CurrentWord = startWord;
        _words = new List<string>(nextWords);
    }

    public void AddWord(string word)
    {
        _words.Add(word);
    }

    public void RemoveWord(string word)
    {
        _words.Remove(word);
    }

    public int WordsAmount(bool incCurrent = true)
    {
        return ((incCurrent && !string.IsNullOrEmpty(CurrentWord)) ? 1 : 0) + _words.Count;
    }

    public bool CycleToNextWord()
    {
        if(string.IsNullOrEmpty(CurrentWord))
        {
            return false;
        }

        string preWord = CurrentWord;
        string nextWord = "";
        if(_words.Count > 0)
        {
            nextWord = _words[0];
            _words.RemoveAt(0);
        }

        CurrentWord = nextWord;

        if (WordCycledEvent != null)
        {
            WordCycledEvent(preWord, CurrentWord);
        }

        return true;
    }

    public bool HasWord(string word, bool incCurrent = true)
    {
        if (incCurrent && CurrentWord == word)
            return true;

        return _words.Contains(word);
    }

    public void Clean()
    {
        _words.Clear();
        _words = null;
        CurrentWord = null;
    }
}
