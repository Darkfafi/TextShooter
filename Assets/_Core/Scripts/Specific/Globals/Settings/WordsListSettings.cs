using UnityEngine;

public class WordsListSettings : ISettings
{
	public const string DEFAULT_FOLDER = "Default";
	public const string DEFAULT_WORDS_LIST_FILE_NAME = "defaultWordsList";

	public string WordsListDocumentText
	{
		get; private set;
	}

	public WordsListSettings()
	{
		Reset();
	}

	public void Reset()
	{
		SetWordsListTextToDefault();
	}

	public void SetWordsListText(string text)
	{
		WordsListDocumentText = text;
	}

	public void SetWordsListTextToDefault()
	{
		SetWordsListText(Resources.Load<TextAsset>(FileUtils.PathToFile(DEFAULT_WORDS_LIST_FILE_NAME, DEFAULT_FOLDER)).text);
	}
}
