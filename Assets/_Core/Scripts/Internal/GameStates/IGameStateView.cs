using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGameStateView<T> where T : class, IGame
{
    void PreStartStateView(GameState<T> state);
    void StartStateView();
    void EndStateView();
}

