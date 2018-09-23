﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameView : MonoBaseView
{
    [SerializeField]
    private IntroGameStateView _introGameStateView;

    [SerializeField]
    private SurvivalGameStateView _survivalGameStateView;

    protected void Start()
    {
        GameModel gm = new GameModel();

        // Setup GameModel
        gm.GameStateManager.SetupStateView<IntroGameState>(_introGameStateView);
        gm.GameStateManager.SetupStateView<SurvivalGameState>(_survivalGameStateView);

        // Create GameModel & GameView as Entity, making them part of the global system
        EntityManager.Instance.LinkAndRegisterEntity(gm, this);
    }
}
