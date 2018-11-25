using UnityEngine;

public abstract class MonoBehaviourGameStateView : MonoBehaviour, IGameStateView<GameModel>
{
	protected GameState<GameModel> GameState
	{
		get; private set;
	}

	public void EndStateView()
	{
		OnEndStateView();
		enabled = false;
		GameState = null;
	}

	public void PreStartStateView(GameState<GameModel> state)
	{
		GameState = state;
		OnPreStartStateView();
		enabled = true;
	}

	public void StartStateView()
	{
		OnStartStateView();
	}

	protected virtual void Awake()
	{
		enabled = false;
	}

	protected abstract void OnPreStartStateView();
	protected abstract void OnStartStateView();
	protected abstract void OnEndStateView();
}
