using UnityEngine;

namespace SurvivalGame
{
	public class WavesGameStateView : MonoGameStateView<SurvivalGameState>
	{
		private WavesGameState _wavesGameState;

		[SerializeField]
		private WaveSystemView _waveSystemView;

		protected override void OnPreStartStateView()
		{
			_wavesGameState = GameState as WavesGameState;

			Controller.Link(_wavesGameState.WaveSystem, _waveSystemView);
		}

		protected override void OnStartStateView()
		{

		}

		protected override void OnEndStateView()
		{
			_wavesGameState = null;
		}
	}
}