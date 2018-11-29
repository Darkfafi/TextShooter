using UnityEngine;

public class GameView : MonoBaseView
{
	[SerializeField]
	private SurvivalGameStateView _survivalGameStateView;

	[Header("Requirements")]
	[SerializeField]
	private CameraView _cameraView;

	private TimekeeperView _timekeeperView;

	protected void Awake()
	{
		_timekeeperView = new GameObject("<TimeKeeperView>").AddComponent<TimekeeperView>();
	}

	protected void Start()
	{
		GameModel gm = new GameModel(_cameraView.Camera.orthographicSize);

		// Setup Camera
		Controller.Link(gm.GameCamera, _cameraView);

		// Setup GameModel
		gm.GameStateManager.SetupStateView<SurvivalGameState>(_survivalGameStateView);

		Controller.Link(gm, this);

		// Setup TimKkeeper
		Controller.Link(gm.TimekeeperModel, _timekeeperView);
	}
}
