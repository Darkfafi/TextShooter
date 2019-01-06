using UnityEngine;

namespace SurvivalGame
{
	public class CampaignGameStateView : MonoGameStateView<SurvivalGameState>
	{
		private CampaignGameState _campaignGameState;

		[SerializeField]
		private TextAsset _campaignXml;

		protected override void OnPreStartStateView()
		{
			_campaignGameState = GameState as CampaignGameState;
			_campaignGameState.SetCampaignString(_campaignXml.text);
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