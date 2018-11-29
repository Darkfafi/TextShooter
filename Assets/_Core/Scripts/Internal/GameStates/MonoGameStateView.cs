using UnityEngine;

public abstract class MonoGameStateView<T> : MonoBehaviour, IGameStateView<T> where T : class, IGame
{
	protected GameState<T> GameState
	{
		get; private set;
	}

	public void EndStateView()
	{
		OnEndStateView();
		enabled = false;
		GameState = null;
	}

	public void PreStartStateView(GameState<T> state)
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
