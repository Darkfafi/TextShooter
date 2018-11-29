using UnityEngine;

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

	private SurvivalGameState _survivalGameState;

	protected override void OnPreStartStateView()
	{
		_survivalGameState = GameState as SurvivalGameState;

		// UI
		Controller.Link(_survivalGameState.WordsDisplayerModel, _wordsDisplayerView);

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
