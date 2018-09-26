using System;
using System.Collections.Generic;

public class GameStateManager<T> where T : class, IGame
{
    private Stack<Type> _gameStateHistory = new Stack<Type>();
    private Dictionary<Type, IGameStateView<T>> _viewToGameState = new Dictionary<Type, IGameStateView<T>>();
    private GameState<T> _currentGameState;
    private IGameStateView<T> _currentView;
    private T _game;

    public GameStateManager(T game)
    {
        _game = game;
    }

    public void SetupStateView<U>(IGameStateView<T> view) where U : GameState<T>
    {
        Type t = typeof(U);
        if(_viewToGameState.ContainsKey(t))
        {
            _viewToGameState[t] = view;
        }
        else
        {
            _viewToGameState.Add(t, view);
        }
    }

    public void SetGameState<U>() where U : GameState<T>
    {
        SetGameState(typeof(U));
    }

    public bool IsCurrentGameState<U>() where U : GameState<T>
    {
        return _currentGameState.GetType() == typeof(U);
    } 

    public void Clean()
    {
        SetGameState(null);
        _gameStateHistory.Clear();
        _viewToGameState.Clear();
        _gameStateHistory = null;
        _viewToGameState = null;
        _game = null;
    }

    private void SetGameState(Type gameStateType)
    {
        if (_currentGameState != null)
        {
            if(_currentView != null)
            {
                _currentView.EndStateView();
                _currentView = null;
            }

            _currentGameState.EndState();
            _gameStateHistory.Push(_currentGameState.GetType());
            _currentGameState = null;
        }

        if (gameStateType == null)
            return;

        GameState<T> gs = Activator.CreateInstance(gameStateType) as GameState<T>;
        _currentGameState = gs;
        _currentGameState.SetupState(_game, this);

        if (_viewToGameState.TryGetValue(gameStateType, out _currentView))
        {
            _currentView.PreStartStateView(_currentGameState);
        }

        _currentGameState.StartState();

        if(_currentView != null)
        {
            _currentView.StartStateView();
        }
    }
}

public interface IGame { }