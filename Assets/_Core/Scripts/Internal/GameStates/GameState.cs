﻿public abstract class GameState<T> where T : class, IGame
{
	public MethodPermitter MethodPermitter
	{
		get; private set;
	}

	public T ParentGame
	{
		get; private set;
	}

	protected GameStateManager<T> GameStateManager
	{
		get; private set;
	}

	public void SetupState(T game, GameStateManager<T> gameStateManager)
	{
		MethodPermitter = new MethodPermitter();
		ParentGame = game;
		GameStateManager = gameStateManager;
		OnSetupState();
	}

	public void StartState()
	{
		OnStartState();
	}

	public void EndState()
	{
		OnEndState();
	}

	protected abstract void OnSetupState();
	protected abstract void OnStartState();
	protected abstract void OnEndState();
}