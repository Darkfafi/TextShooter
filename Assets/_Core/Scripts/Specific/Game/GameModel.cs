using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameModel : EntityModel
{
    public GameStateManager GameStateManager { get; private set; }

    public GameModel()
    {
        GameStateManager = new GameStateManager();
    }

    protected override void OnEntityReady()
    {
        Debug.Log("Hello World!");
        GameStateManager.SetGameState<IntroGameState>();
    }
}
