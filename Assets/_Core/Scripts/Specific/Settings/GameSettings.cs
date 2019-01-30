using UnityEngine;

public class GameSettings : ISettings
{
	public const string DEFAULT_FOLDER = "Default";
	public const string DEFAULT_CAMPAIGN_FILE_NAME = "defaultCampaign";
	public const string DEFAULT_WORDS_LIST_FILE_NAME = "defaultWordsList";

	public string WordsListDocumentText
	{
		get; private set;
	}

	public string CampaignText
	{
		get; private set;
	}

	public GameSettings()
	{
		Reset();
	}

	public void Reset()
	{
		SetWordsListTextToDefault();
		SetCampaignTextToDefault();
	}

	public void SetWordsListText(string text)
	{
		WordsListDocumentText = text;
	}

	public void SetCampaignText(string text)
	{
		CampaignText = text;
	}

	public void SetCampaignTextToDefault()
	{
		SetCampaignText(Resources.Load<TextAsset>(GetFolderToDefaultFile(DEFAULT_CAMPAIGN_FILE_NAME)).text);
	}

	public void SetWordsListTextToDefault()
	{
		SetWordsListText(Resources.Load<TextAsset>(GetFolderToDefaultFile(DEFAULT_WORDS_LIST_FILE_NAME)).text);
	}

	private string GetFolderToDefaultFile(string fileName, params string[] pathParts)
	{
		string s = DEFAULT_FOLDER;
		for(int i = 0; i < pathParts.Length; i++)
		{
			s += "/" + pathParts[i];
		}
		return s + "/" + fileName;
	}
}
