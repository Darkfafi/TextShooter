using UnityEngine;

public class GameView : MonoBaseView
{
    [SerializeField]
    private IntroGameStateView _introGameStateView;

    [SerializeField]
    private SurvivalGameStateView _survivalGameStateView;

    private TimekeeperView _timekeeperView;

    protected void Awake()
    {
        _timekeeperView = new GameObject("<TimeKeeperView>").AddComponent<TimekeeperView>();
    }

    protected void Start()
    {
        GameModel gm = new GameModel();

        // Setup GameModel
        gm.GameStateManager.SetupStateView<IntroGameState>(_introGameStateView);
        gm.GameStateManager.SetupStateView<SurvivalGameState>(_survivalGameStateView);

        Controller<GameModel, GameView>.Link(gm, this);

        // Setup TimKkeeper
        Controller<TimekeeperModel, TimekeeperView>.Link(gm.TimekeeperModel, _timekeeperView);
    }
}
