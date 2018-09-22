using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroGameStateView : MonoBehaviourGameStateView
{
    private IntroGameState _introGameState;

    protected override void OnPreStartStateView()
    {
        _introGameState = GameState as IntroGameState;
    }

    protected override void OnStartStateView()
    {

    }

    protected override void OnEndStateView()
    {
        _introGameState = null;
    }
}
