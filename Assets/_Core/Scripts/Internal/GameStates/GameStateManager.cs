using System;
using System.Collections.Generic;

public class GameStateManager
{
    private Stack<Type> _gameStateHistory = new Stack<Type>();
    private Dictionary<Type, IGameStateView> _viewToGameState = new Dictionary<Type, IGameStateView>();
    private GameState _currentGameState;
    private IGameStateView _currentView;

    public void SetupStateView<T>(IGameStateView view) where T : GameState
    {
        Type t = typeof(T);
        if(_viewToGameState.ContainsKey(t))
        {
            _viewToGameState[t] = view;
        }
        else
        {
            _viewToGameState.Add(t, view);
        }
    }

    public void SetGameState<T>() where T : GameState
    {
        SetGameState(typeof(T));
    }

    public bool IsCurrentGameState<T>() where T : GameState
    {
        return _currentGameState.GetType() == typeof(T);
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

        GameState gs = Activator.CreateInstance(gameStateType) as GameState;
        _currentGameState = gs;
        _currentGameState.SetupState();

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