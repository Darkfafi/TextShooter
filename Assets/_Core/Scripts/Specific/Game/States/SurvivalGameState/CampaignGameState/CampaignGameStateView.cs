namespace SurvivalGame
{
	public class CampaignGameStateView : MonoGameStateView<SurvivalGameState>
	{
		private CampaignGameState _campaignGameState;

		protected override void OnPreStartStateView()
		{
			_campaignGameState = GameState as CampaignGameState;
		}

		protected override void OnStartStateView()
		{

		}

		protected override void OnEndStateView()
		{
			_campaignGameState = null;
		}
	}
}