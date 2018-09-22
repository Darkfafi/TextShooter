using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurvivalGameState : GameState
{
    public WaveSystemModel WaveSystem { get; private set; }

    public SurvivalGameState()
    {
        WaveSystem = new WaveSystemModel();
    }

    protected override void OnSetupState()
    {
        //EntityManager.Instance.GetAnEntity<GameModel>(); <--- Can be used to get to game specific data
        // Though, track all SurvivalGameState data in this Game state. 
    }

    protected override void OnStartState()
    {

    }

    protected override void OnEndState()
    {

    }
}
