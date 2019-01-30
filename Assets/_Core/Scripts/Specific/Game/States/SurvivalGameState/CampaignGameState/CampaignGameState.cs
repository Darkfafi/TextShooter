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
			Campaign = XmlToCampaign.ParseXml(MasterGame, MasterGame.GameSettings.CampaignText);
		}

		protected override void OnStartState()
		{
			Campaign.StartCampaign();
		}

		protected override void OnEndState()
		{
			Campaign.EndCampaign();
			Campaign = null;
		}
	}
}
