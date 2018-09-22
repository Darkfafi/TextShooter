using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameState
{
    protected GameStateManager GameStateManager { get; private set; }

    public void SetupState(GameStateManager gameStateManager)
    {
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