using System;
using UnityEngine;

namespace Rules.Campaign
{
	public class CampaignRules
	{
		public RequirementInfo[] Requirements;

		private static CampaignRules _instance;

		public static CampaignRules GetRules()
		{
			if(_instance == null)
			{
				_instance = JsonUtility.FromJson<CampaignRules>(ResourceLocator.Locate<TextAsset>("campaignRules", "Rules").text);
			}

			return _instance;
		}

		private CampaignRules()
		{

		}
	}

	[Serializable]
	public struct RequirementInfo
	{
		public string Name;
		public string ValueType;
		public string ValueOptions;
		public string Description;
	}
}