using UnityEngine;

public class SurvivalGameStateView : MonoBehaviourGameStateView
{
    private SurvivalGameState _survivalGameState;

    [SerializeField]
    private WaveSystemView _waveSystemView;

    protected override void OnPreStartStateView()
    {
        _survivalGameState = GameState as SurvivalGameState;
        Controller.Link(_survivalGameState.WaveSystem, _waveSystemView);
        // Setup Game Visual Elements and link them to their respective Model

        _waveSystemView.StartWaveSystem();
    }

    protected override void OnStartStateView()
    {

    }

    protected override void OnEndStateView()
    {
        _survivalGameState = null;
    }
}
