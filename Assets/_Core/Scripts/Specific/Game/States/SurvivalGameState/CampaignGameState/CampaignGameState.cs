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
				Campaign = XmlToCampaign.ParseXml(MasterGame, campaignXmlString);
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
