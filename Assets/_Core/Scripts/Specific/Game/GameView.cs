using UnityEngine;
using SurvivalGame;

public class GameView : MonoBaseView
{
	[SerializeField]
	private SurvivalGameStateView _survivalGameStateView;

	[Header("Requirements")]
	[SerializeField]
	private CameraView _cameraView;

	private TimekeeperView _timekeeperView;
	private GameModel _gameModel;

	protected void Awake()
	{
		_timekeeperView = new GameObject("<TimeKeeperView>").AddComponent<TimekeeperView>();
	}

	protected void OnDestroy()
	{
		_gameModel.Destroy();
		_gameModel = null;
	}

	protected void Start()
	{
		_gameModel = new GameModel(_cameraView.Camera.orthographicSize);

		// Setup Camera
		Controller.Link(_gameModel.GameCamera, _cameraView);

		// Setup TimKkeeper
		Controller.Link(_gameModel.TimekeeperModel, _timekeeperView);

		// Setup GameModel
		_gameModel.GameStateManager.SetupStateView<SurvivalGameState>(_survivalGameStateView);

		Controller.Link(_gameModel, this);
	}
}
