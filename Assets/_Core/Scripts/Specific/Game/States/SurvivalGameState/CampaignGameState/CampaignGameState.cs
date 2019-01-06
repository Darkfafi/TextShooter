using System;
using System.Xml;

namespace SurvivalGame
{
	public class CampaignGameState : SubGameState<SurvivalGameState, GameModel>
	{
		public Timeline<GameModel> Timeline
		{
			get; private set;
		}

		public string CampaignXmlString
		{
			get; private set;
		}

		protected override void OnSetupState()
		{

		}

		public void SetCampaignString(string campaignXmlString)
		{
			if(string.IsNullOrEmpty(CampaignXmlString) && !string.IsNullOrEmpty(campaignXmlString))
			{
				CampaignXmlString = campaignXmlString;
				XmlDocument xmlDoc = new XmlDocument();
				xmlDoc.LoadXml(campaignXmlString);

				foreach(XmlNode node in xmlDoc.DocumentElement.ChildNodes)
				{
					if(node.Name == "timeline")
					{

						Timeline = XmlToTimeline.ParseXml(MasterGame, node, GetDataParserForType);
					}
				}
			}
		}

		private BaseTimelineEventDataParser GetDataParserForType(string eventType)
		{
			switch(eventType)
			{
				case "mobs":
					return new MobsDataParser();
			}

			return null;
		}

		protected override void OnStartState()
		{
			Timeline.TimelineEndReachedEvent += OnEndReachedEvent;
			Timeline.TimelineEventEndedEvent += OnTimelineEventEndedEvent;

			// Start First Timeline Event
			Timeline.SetNewTimelinePosition(0);
		}

		protected override void OnEndState()
		{
			Timeline.TimelineEndReachedEvent -= OnEndReachedEvent;
			Timeline.TimelineEventEndedEvent -= OnTimelineEventEndedEvent;
		}

		private void OnTimelineEventEndedEvent(IReadableTimelineEvent timelineEvent)
		{
			UnityEngine.Debug.LogFormat("End of event {0} reached!", timelineEvent.GetType().ToString());
			Timeline.Up();
		}

		private void OnEndReachedEvent()
		{
			UnityEngine.Debug.Log("End of timeline reached!");
		}
	}
}
