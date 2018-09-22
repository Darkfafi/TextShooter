using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGameStateView
{
    void PreStartStateView(GameState state);
    void StartStateView();
    void EndStateView();
}

