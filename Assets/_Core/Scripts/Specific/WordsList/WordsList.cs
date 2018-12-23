using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class WordsList 
{
	public WordsListData ListData
	{
		get; private set;
	}

	public WordsList(string wordsListDocumentContent)
	{
		SetWordsListContent(wordsListDocumentContent);
	}

	public static bool ValidateContent(string wordsListDocumentContent, out string errorMessage)
	{
		errorMessage = null;
		try
		{
			return ValidateData(CreateWordsListData(wordsListDocumentContent), out errorMessage);
		}
		catch(Exception e)
		{
			errorMessage = e.Message;
			return false;
		}
	}

	public static bool ValidateContent(TextAsset document, out string errorMessage)
	{
		return ValidateContent(document.text, out errorMessage);
	}

	public static bool ValidateContent(TextAsset document)
	{
		return ValidateContent(document.text);
	}

	public static bool ValidateContent(string wordsListDocumentContent)
	{
		string errorMessage = null;
		bool validation = ValidateContent(wordsListDocumentContent, out errorMessage);

		if(errorMessage != null)
		{
			Debug.LogError("WordsList Error Message: " + errorMessage);
		}

		return validation;
	}

	public static bool ValidateData(WordsListData data)
	{
		string errorMessage = null;
		bool validation = ValidateData(data, out errorMessage);

		if(errorMessage != null)
		{
			Debug.LogError("WordsListData Error Message: " + errorMessage);
		}

		return validation;
	}

	public static bool ValidateData(WordsListData data, out string errorMessage)
	{
		errorMessage = null;
		if(string.IsNullOrEmpty(data.Title))
		{
			errorMessage = "Title must have a value!";
			return false;
		}

		if(data.Words == null || data.Words.Count < WordsListGlobals.MINIMUM_WORDS_AMOUNT)
		{
			errorMessage = string.Format("List must contain at least {0} words! Currently contains {1}!", WordsListGlobals.MINIMUM_WORDS_AMOUNT, data.Words == null ? 0 : data.Words.Count);
			return false;
		}

		for(int i = 0, c = data.Words.Count; i < c; i++)
		{
			string word = data.Words[i];
			if(string.IsNullOrEmpty(word) || word.Length > WordsListGlobals.MAX_WORD_LENGTH)
			{
				if(word == null)
					word = "";

				errorMessage = string.Format("Word '{0}' must contain a value and must be under {1} characters long, currently contains {2} characters!", word, WordsListGlobals.MAX_WORD_LENGTH, word.Length);
				return false;
			}

			Regex regex = new Regex(@"^(\w+)$");
			Match match = regex.Match(word);
			if(!match.Success)
			{
				errorMessage = string.Format("Word '{0}' may only contain alphanumeric characters!", word);
				return false;
			}
		}

		return true;
	}

	public void SetWordsListContent(string wordsListDocumentContent)
	{
		try
		{
			ListData = CreateWordsListData(wordsListDocumentContent);
		}
		catch(Exception e)
		{
			Debug.LogError("WordsList Error Message: " + e.Message);
		}
	}

	public static WordsListData CreateWordsListData(string wordsListDocumentContent)
	{
		WordsListData buildingList = new WordsListData("");
		string[] lines = wordsListDocumentContent.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
		string[] lineSaparator = new string[] { "->" };
		for(int i = 0, c = lines.Length; i < c; i++)
		{
			// Line in the document.
			string line = lines[i];

			if(string.IsNullOrEmpty(line))
			{
				continue;
			}

			string[] lineSections = line.Split(lineSaparator, StringSplitOptions.None);
			if(lineSections.Length == 2)
			{
				ReadOutLine(lineSections[0], lineSections[1], ref buildingList);
			}
			else
			{
				throw new Exception(string.Format("Line '{0}' devided in sections using '{1}' was not valid", line, lineSaparator[0]));
			}
		}

		return buildingList;
	}

	public static void ReadOutLine(string type, string value, ref WordsListData data)
	{
		switch(type)
		{
			case WordsListGlobals.TYPE_TITLE:
				data.Title = value;
				break;
			case WordsListGlobals.TYPE_WORD:
				data.Words.Add(value);
				break;
			default:
				throw new Exception(string.Format("Type {0} is not supported by the WordsList", type));
		}
	}
}

public struct WordsListData
{
	public string Title;
	public List<string> Words;

	public WordsListData(string title, params string[] words)
	{
		Title = title;
		Words = new List<string>(words);
	}
}