using System.Collections.Generic;

public class WordsHolder : BaseModelComponent
{
	public delegate void PreNewWordHandler(string previousWord, string newWord, WordsHolder wordsHolder);
	public event PreNewWordHandler WordCycledEvent;

	public string CurrentWord
	{
		get; private set;
	}
	private List<string> _words = new List<string>();

	private bool _setup = false;

	public void Setup(string startWord, params string[] nextWords)
	{
		if(_setup)
			return;

		CurrentWord = startWord;
		_words = new List<string>(nextWords);
		_setup = true;

		if(WordCycledEvent != null)
		{
			WordCycledEvent("", CurrentWord, this);
		}
	}

	protected override void Removed()
	{
		_words.Clear();
		_words = null;
		CurrentWord = null;
	}

	public void AddWord(string word)
	{
		_words.Add(word);
	}

	public void RemoveWord(string word)
	{
		_words.Remove(word);
	}

	public int GetWordsAmount(bool incCurrent = true)
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

		if(WordCycledEvent != null)
		{
			WordCycledEvent(preWord, CurrentWord, this);
		}

		return true;
	}

	public bool HasWord(string word, bool incCurrent = true)
	{
		if(incCurrent && CurrentWord == word)
			return true;

		return _words.Contains(word);
	}
}
