using UnityEngine;

public class SurvivalGameStateView : MonoBehaviourGameStateView
{
	private SurvivalGameState _survivalGameState;

	[SerializeField]
	private WaveSystemView _waveSystemView;

	[SerializeField]
	private TurretView _turretView;

	[SerializeField]
	private WordsDisplayerView _wordsDisplayerView;

	protected override void OnPreStartStateView()
	{
		_survivalGameState = GameState as SurvivalGameState;

		Controller.Link(_survivalGameState.TurretModel, _turretView);
		Controller.Link(_survivalGameState.WordsDisplayerModel, _wordsDisplayerView);
		Controller.Link(_survivalGameState.WaveSystem, _waveSystemView);
	}

	protected override void OnStartStateView()
	{

	}

	protected override void OnEndStateView()
	{
		_survivalGameState = null;
	}
}
