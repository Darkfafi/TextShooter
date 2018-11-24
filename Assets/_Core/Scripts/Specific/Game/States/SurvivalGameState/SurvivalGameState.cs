using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurvivalGameState : BaseGameState
{
    public WaveSystemModel WaveSystem { get; private set; }
    public TurretModel TurretModel { get; private set; }
	public WordsDisplayerModel WordsDisplayerModel { get; private set; }

	protected override void OnSetupState()
    {
        // Setup Player
        TurretModel = new TurretModel(Game.TimekeeperModel);

		// Setup Words Displayer
		WordsDisplayerModel = new WordsDisplayerModel(Game.TimekeeperModel);

        // Setup Environment
        WaveSystem = new WaveSystemModel(Game.GameCamera, Game.TimekeeperModel);
    }

    protected override void OnStartState()
    {
        WaveSystem.StartWaveSystem();
    }

    protected override void OnEndState()
    {

    }
}
