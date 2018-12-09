using UnityEngine;

namespace SurvivalGame
{
	public class SurvivalGameStateView : MonoGameStateView<GameModel>
	{
		[Header("Game States")]
		[SerializeField]
		private IntroGameStateView _introGameStateView;

		[SerializeField]
		private WavesGameStateView _wavesGameStateView;

		[Header("Requirements")]
		[SerializeField]
		private TurretView _turretView;

		[SerializeField]
		private WordsDisplayerView _wordsDisplayerView;

		private CharKeyboardInputView _charKeyboardInputView;

		private SurvivalGameState _survivalGameState;

		protected override void Awake()
		{
			base.Awake();
			_charKeyboardInputView = gameObject.AddComponent<CharKeyboardInputView>();
		}

		protected override void OnPreStartStateView()
		{
			_survivalGameState = GameState as SurvivalGameState;

			// UI
			Controller.Link(_survivalGameState.WordsDisplayerModel, _wordsDisplayerView);

			// Input
			Controller.Link(_survivalGameState.CharInputModel, _charKeyboardInputView);

			// Game
			Controller.Link(_survivalGameState.TurretModel, _turretView);

			// Game States
			_survivalGameState.SurvivalGameStateManager.SetupStateView<IntroGameState>(_introGameStateView);
			_survivalGameState.SurvivalGameStateManager.SetupStateView<WavesGameState>(_wavesGameStateView);
		}

		protected override void OnStartStateView()
		{

		}

		protected override void OnEndStateView()
		{
			_survivalGameState = null;
		}
	}
}