using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameModel : BaseModel
{
    public GameStateManager GameStateManager { get; private set; }

    public GameModel()
    {
        GameStateManager = new GameStateManager();
    }

    protected override void OnEntityReady()
    {
        GameStateManager.SetGameState<IntroGameState>();
    }
}
