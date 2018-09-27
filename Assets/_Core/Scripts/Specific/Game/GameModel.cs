public class GameModel : BaseModel, IGame
{
    public CameraModel GameCamera { get; private set; }
    public GameStateManager<GameModel> GameStateManager { get; private set; }
    public TimekeeperModel TimekeeperModel { get; private set; }

    public GameModel(float orthographicSize)
    {
        GameCamera = new CameraModel(orthographicSize);
        GameStateManager = new GameStateManager<GameModel>(this);
        TimekeeperModel = new TimekeeperModel();
    }

    protected override void OnModelReady()
    {
        GameStateManager.SetGameState<IntroGameState>();
    }

    protected override void OnModelDestroy()
    {
        GameStateManager.Clean();
        TimekeeperModel.Destroy();
        GameStateManager = null;
        TimekeeperModel = null;
    }
}
