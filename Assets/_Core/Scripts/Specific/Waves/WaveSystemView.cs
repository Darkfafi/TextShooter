using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSystemView : EntityView
{
    [SerializeField]
    private Camera _gameCamera;

    [SerializeField]
    private float _orthographicSpawnMargin = 1f;

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
        float spawnDistY = _gameCamera.orthographicSize + _orthographicSpawnMargin;
        float spawnDistX = spawnDistY * Screen.width / Screen.height;
        for (int i = 0; i < waveSection.Enemies.Length * 100; i++)
        {
            float distanceVarienceValue = UnityEngine.Random.value * 2f;
            bool fullX = UnityEngine.Random.value > 0.5f;
            int xMult = UnityEngine.Random.value > 0.5f ? 1 : -1;
            int yMult = UnityEngine.Random.value > 0.5f ? 1 : -1;
            float x = ((fullX) ? 1 : UnityEngine.Random.value);
            float y = ((!fullX) ? 1 : UnityEngine.Random.value);
            x = (Mathf.Lerp(0, spawnDistX, x) + distanceVarienceValue) * xMult;
            y = (Mathf.Lerp(0, spawnDistY, y) + distanceVarienceValue) * yMult;
            Vector2 spawnPos = new Vector2(x, y);

            // Test Object Spawn
            GameObject go = new GameObject("Test Enemy");
            go.transform.position = spawnPos;
        }

        endOfWaveSectionCallback(waveSectionIndex);
    }
}
