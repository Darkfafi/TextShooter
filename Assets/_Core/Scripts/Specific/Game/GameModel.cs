using SurvivalGame;

public class GameModel : BaseModel, IGame
{
	public CameraModel GameCamera
	{
		get; private set;
	}

	public TimekeeperModel TimekeeperModel
	{
		get; private set;
	}

	public GameFactories Factories
	{
		get; private set;
	}

	public GameStateManager<GameModel> GameStateManager
	{
		get; private set;
	}

	public GameSettings GameSettings
	{
		get; private set;
	}

	public GameModel(float orthographicSize)
	{
		GameSettings = SessionSettings.Request<GameSettings>();
		GameCamera = new CameraModel(orthographicSize, orthographicSize);
		TimekeeperModel = new TimekeeperModel();
		Factories = new GameFactories(this);
		GameStateManager = new GameStateManager<GameModel>(this);
	}

	protected override void OnModelReady()
	{
		GameStateManager.SetGameState<SurvivalGameState>();
	}

	protected override void OnModelDestroy()
	{
		GameStateManager.Clean();
		TimekeeperModel.Destroy();
		GameCamera.Destroy();
		GameCamera = null;
		GameStateManager = null;
		TimekeeperModel = null;
		Factories = null;
	}
}
