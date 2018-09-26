using UnityEngine;

public class SurvivalGameStateView : MonoBehaviourGameStateView
{
    private SurvivalGameState _survivalGameState;

    [SerializeField]
    private WaveSystemView _waveSystemView;

    [SerializeField]
    private TurretView _turretView;

    protected override void OnPreStartStateView()
    {
        _survivalGameState = GameState as SurvivalGameState;

        Controller.Link(_survivalGameState.TurretModel, _turretView);
        Controller.Link(_survivalGameState.WaveSystem, _waveSystemView);

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
