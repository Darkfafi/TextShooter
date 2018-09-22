using System.Collections.Generic;

public class WordsHolder
{
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
        return (incCurrent ? 1 : 0) + _words.Count;
    }

    public bool CycleToNextWord()
    {
        if (_words.Count > 0)
        {
            CurrentWord = _words[0];
            _words.RemoveAt(0);
            return true;
        }

        return false;
    }

    public bool HasWord(string word, bool incCurrent = true)
    {
        if (incCurrent && CurrentWord == word)
            return true;

        return _words.Contains(word);
    }
}
