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

	public FactoryHolder Factories
	{
		get; private set;
	}

	public GameStateManager<GameModel> GameStateManager
	{
		get; private set;
	}

	public GameModel(float orthographicSize)
	{
		GameCamera = new CameraModel(orthographicSize, orthographicSize);
		TimekeeperModel = new TimekeeperModel();

		// Factories
		Factories = new FactoryHolder();

		GameStateManager = new GameStateManager<GameModel>(this);
	}

	protected override void OnModelReady()
	{
		GameStateManager.SetGameState<SurvivalGameState>();
	}

	protected override void OnModelDestroy()
	{
		GameCamera.Destroy();
		GameStateManager.Clean();
		Factories.Clean();
		TimekeeperModel.Destroy();
		GameCamera = null;
		GameStateManager = null;
		TimekeeperModel = null;
		Factories = null;
	}
}
