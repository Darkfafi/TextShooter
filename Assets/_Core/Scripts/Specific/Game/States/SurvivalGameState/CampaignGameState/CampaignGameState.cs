using System.Xml;

namespace SurvivalGame
{
	public class CampaignGameState : SubGameState<SurvivalGameState, GameModel>
	{
		public Campaign<GameModel> Campaign
		{
			get; private set;
		}

		protected override void OnSetupState()
		{

		}

		public void SetCampaignString(string campaignXmlString)
		{
			if(Campaign == null)
			{
				string name = "", description = "";
				Timeline<GameModel> timeline = null;
				XmlDocument xmlDoc = new XmlDocument();
				xmlDoc.LoadXml(campaignXmlString);

				foreach(XmlNode node in xmlDoc.DocumentElement.ChildNodes)
				{
					if(node.Name == "name")
					{
						name = node.InnerText;
					}

					if(node.Name == "description")
					{
						description = node.InnerText;
					}

					if(node.Name == "timeline")
					{

						timeline = XmlToTimeline.ParseXml(MasterGame, node, EventDataParsers.GetDataParserForType);
					}
				}

				Campaign = new Campaign<GameModel>(new CampaignData()
				{
					Name = name,
					Description = description
				}, timeline);
			}
		}

		protected override void OnStartState()
		{
			Campaign.StartCampaign();
		}

		protected override void OnEndState()
		{
			Campaign.EndCampaign();
		}
	}
}
