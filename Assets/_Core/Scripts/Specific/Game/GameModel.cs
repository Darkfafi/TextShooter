﻿using SurvivalGame;

public class GameModel : BaseModel, IGame
{
	public CameraModel GameCamera
	{
		get; private set;
	}

	public GameStateManager<GameModel> GameStateManager
	{
		get; private set;
	}

	public TimekeeperModel TimekeeperModel
	{
		get; private set;
	}

	public GameModel(float orthographicSize)
	{
		GameCamera = new CameraModel(orthographicSize, orthographicSize);
		GameStateManager = new GameStateManager<GameModel>(this);
		TimekeeperModel = new TimekeeperModel();
	}

	protected override void OnModelReady()
	{
		GameStateManager.SetGameState<SurvivalGameState>();
	}

	protected override void OnModelDestroy()
	{
		GameStateManager.Clean();
		TimekeeperModel.Destroy();
		GameStateManager = null;
		TimekeeperModel = null;
	}
}
