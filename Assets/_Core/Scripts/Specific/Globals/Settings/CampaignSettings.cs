using UnityEngine;

public class CampaignSettings : ISettings
{
	public const string DEFAULT_FOLDER = "Default";
	public const string DEFAULT_CAMPAIGN_FILE_NAME = "defaultCampaign";

	public string CampaignText
	{
		get; private set;
	}

	public CampaignSettings()
	{
		Reset();
	}

	public void Reset()
	{
		SetCampaignTextToDefault();
	}

	public void SetCampaignText(string text)
	{
		CampaignText = text;
	}

	public void SetCampaignTextToDefault()
	{
		SetCampaignText(Resources.Load<TextAsset>(FileUtils.PathToFile(DEFAULT_CAMPAIGN_FILE_NAME, DEFAULT_FOLDER)).text);
	}
}
