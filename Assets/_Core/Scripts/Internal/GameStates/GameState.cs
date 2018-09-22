using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameState
{
    public void SetupState()
    {
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