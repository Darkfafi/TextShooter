using RDP.SaveLoadSystem;

public static class StorageGlobals
{
	public const string STORAGE_LOCATION = "Saves/";
	public const string CAPSULE_CAMPAIGN_EDITOR = "CampaignEditor";
	public const Storage.EncodingType STORAGE_ENCODING_TYPE = Storage.EncodingType.None;

	public static Storage CreateStorage(params IStorageCapsule[] capsules)
	{
		return new Storage(STORAGE_LOCATION, STORAGE_ENCODING_TYPE, capsules);
	}
}
