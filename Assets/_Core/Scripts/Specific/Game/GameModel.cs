using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameModel : BaseModel, IGame
{
    public GameStateManager<GameModel> GameStateManager { get; private set; }
    public TimekeeperModel TimekeeperModel { get; private set; }

    public GameModel()
    {
        GameStateManager = new GameStateManager<GameModel>(this);
        TimekeeperModel = new TimekeeperModel();
    }

    protected override void OnEntityReady()
    {
        GameStateManager.SetGameState<IntroGameState>();
    }
}
