using RDP.SaveLoadSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameEditor
{
	public class CampaignEditorFile : ISaveable
	{
		// Save / Load Keys
		public const string STORAGE_EVENT_NODES_SLOTS_KEY = "EventNodesSlotsKey";
		public const string STORAGE_CAMPAIGN_NAME_KEY = "CampaignNameKey";
		public const string STORAGE_CAMPAIGN_DESCRIPTION_KEY = "CampaignDescriptionKey";

		// Consts
		public const string DEFAULT_CAMPAIGN_NAME = "Default";
		public const string DEFAULT_CAMPAIGN_DESCRIPTION = "An exciting and challenging campaign to play!";

		public string CampaignName
		{
			get; private set;
		}

		public string CampaignDescription
		{
			get; private set;
		}

		public List<EventNodesSlotModel> _eventNodesSlots = new List<EventNodesSlotModel>();

		public CampaignEditorFile(string name = DEFAULT_CAMPAIGN_NAME, string description = DEFAULT_CAMPAIGN_DESCRIPTION)
		{
			CampaignName = name;
			CampaignDescription = description;
			LoadingCompleted();
		}

		public CampaignEditorFile(IStorageLoader loader)
		{
			CampaignName = loader.LoadValue<string>(STORAGE_CAMPAIGN_NAME_KEY);
			CampaignDescription = loader.LoadValue<string>(STORAGE_CAMPAIGN_DESCRIPTION_KEY);
			loader.LoadRefs<EventNodesSlotModel>(STORAGE_EVENT_NODES_SLOTS_KEY, (instances) => _eventNodesSlots.AddRange(instances));
		}

		public void Save(IStorageSaver saver)
		{
			saver.SaveValue(STORAGE_CAMPAIGN_NAME_KEY, CampaignName);
			saver.SaveValue(STORAGE_CAMPAIGN_DESCRIPTION_KEY, CampaignDescription);
			saver.SaveRefs(STORAGE_EVENT_NODES_SLOTS_KEY, _eventNodesSlots.ToArray(), false);
		}

		public void LoadingCompleted()
		{
			if(_eventNodesSlots.Count == 0)
				_eventNodesSlots.Add(new EventNodesSlotModel());
		}
	}
}