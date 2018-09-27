using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurvivalGameState : BaseGameState
{
    public WaveSystemModel WaveSystem { get; private set; }
    public TurretModel TurretModel { get; private set; }

    protected override void OnSetupState()
    {
        // Setup Player
        TurretModel = new TurretModel(Game.TimekeeperModel);

        // Setup Environment
        WaveSystem = new WaveSystemModel(Game.GameCamera, Game.TimekeeperModel);
    }

    protected override void OnStartState()
    {
        WaveSystem.StartWaveSystem();
        TurretModel.FocusOnTarget(EntityTracker.Instance.GetAnEntity<EnemyModel>()); //TODO: Replace Test with real targeting code
    }

    protected override void OnEndState()
    {

    }
}
