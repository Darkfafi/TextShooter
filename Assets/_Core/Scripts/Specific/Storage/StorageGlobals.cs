using RDP.SaveLoadSystem;

public static class StorageGlobals
{
	public const string STORAGE_LOCATION = "Saves/";
	public const string STORAGE_LOCATION_CAMPAIGN_EDITOR_FILES = STORAGE_LOCATION + "CampaignEditorFiles/";

	public const Storage.EncodingType STORAGE_ENCODING_TYPE = Storage.EncodingType.None;

	public static Storage CreateStorage(params IStorageCapsule[] capsules)
	{
		return new Storage(STORAGE_LOCATION, STORAGE_ENCODING_TYPE, capsules);
	}

	public static Storage CreateCampaignEditorStorage(params IStorageCapsule[] capsules)
	{
		return new Storage(STORAGE_LOCATION_CAMPAIGN_EDITOR_FILES, STORAGE_ENCODING_TYPE, capsules);
	}
}
