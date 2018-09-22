using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSystemView : EntityView
{
    private EnemyViewFactory _enemyViewFactory;
    private WaveSystemModel _waveSystemModel;

    public void StartWaveSystem(int startWave = 0)
    {
        SpawnWave(startWave);
    }

    protected void Awake()
    {
        _enemyViewFactory = new EnemyViewFactory();
    }

    protected override void OnViewReady()
    {
        _waveSystemModel = MVCUtil.GetModel<WaveSystemModel>(this);
    }

    private void SpawnNextWave()
    {
        SpawnWave(_waveSystemModel.CurrentWave + 1);
    }

    private void SpawnWave(int wave)
    {
        WaveSystemModel.WaveInfo waveInfo = _waveSystemModel.SpawnWave(wave);
        WaveSectionLoop(waveInfo, -1); // Start at the first section
    }

    private void WaveSectionLoop(WaveSystemModel.WaveInfo waveInfo, int currentWaveSectionIndex)
    {
        int nextWaveSection = currentWaveSectionIndex + 1;
        if (nextWaveSection >= waveInfo.WaveSections.Length)
        {
            // End Wave
            Debug.Log("TODO: END WAVE, NEXT WAVE LOGICS HERE");
        }
        else
        {
            SpawnWaveSection(waveInfo.WaveSections[nextWaveSection], nextWaveSection, (index)=> 
            {
                WaveSectionLoop(waveInfo, index);
            });
        }
    }

    private void SpawnWaveSection(WaveSystemModel.WaveSectionObject waveSection, int waveSectionIndex, Action<int> endOfWaveSectionCallback)
    {
        Debug.Log("TODO: SPAWN SECTION LOGICS HERE");
        endOfWaveSectionCallback(waveSectionIndex);
    }
}
