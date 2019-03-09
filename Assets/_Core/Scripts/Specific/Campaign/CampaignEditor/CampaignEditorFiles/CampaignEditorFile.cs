using RDP.SaveLoadSystem;
using System.Collections.Generic;
using System;

namespace GameEditor
{
	public class CampaignEditorFile : IStorageCapsule
	{
		// Save / Load Keys
		public const string STORAGE_EVENT_NODES_SLOTS_KEY = "EventNodesSlotsKey";
		public const string STORAGE_CAMPAIGN_NAME_KEY = "CampaignNameKey";
		public const string STORAGE_CAMPAIGN_DESCRIPTION_KEY = "CampaignDescriptionKey";

		// Consts
		public const string DEFAULT_CAMPAIGN_NAME = "Default";
		public const string DEFAULT_CAMPAIGN_DESCRIPTION = "An exciting and challenging campaign to play!";

		public string ID
		{
			get; private set;
		}

		public string CampaignName;
		public string CampaignDescription;
		public List<EventNodesSlotModel> EventNodesSlots = new List<EventNodesSlotModel>();

		public static CampaignEditorFile CreateNew()
		{
			return new CampaignEditorFile(Guid.NewGuid().ToString("N"));
		}

		public CampaignEditorFile(string storageID)
		{
			ID = storageID;
			CampaignName = DEFAULT_CAMPAIGN_NAME;
			CampaignDescription = DEFAULT_CAMPAIGN_DESCRIPTION;
		}

		public void Unload()
		{
			for(int i = 0; i < EventNodesSlots.Count; i++)
			{
				EventNodesSlots[i].Destroy();
			}

			EventNodesSlots.Clear();
		}

		public void Save(IStorageSaver saver)
		{
			saver.SaveValue(STORAGE_CAMPAIGN_NAME_KEY, CampaignName);
			saver.SaveValue(STORAGE_CAMPAIGN_DESCRIPTION_KEY, CampaignDescription);
			saver.SaveRefs(STORAGE_EVENT_NODES_SLOTS_KEY, EventNodesSlots.ToArray(), false);
		}

		public void Load(IStorageLoader loader)
		{
			CampaignName = loader.LoadValue<string>(STORAGE_CAMPAIGN_NAME_KEY);
			CampaignDescription = loader.LoadValue<string>(STORAGE_CAMPAIGN_DESCRIPTION_KEY);
			loader.LoadRefs<EventNodesSlotModel>(STORAGE_EVENT_NODES_SLOTS_KEY, (instances) => EventNodesSlots.AddRange(instances));
		}

		public void LoadingCompleted()
		{

		}
	}
}