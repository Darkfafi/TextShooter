using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSystemView : MonoBaseView
{
    [Header("Options")]
    [SerializeField]
    private float _orthographicSpawnMargin = 1f;

    [Header("Requirements")]
    [SerializeField]
    private Camera _gameCamera;

    [SerializeField]
    private EnemyViewFactory _enemyViewFactory;

    private WaveSystemModel _waveSystemModel;
    private Coroutine _currentCoroutine;

    #region LifeCycle

    protected override void OnViewReady()
    {
        _waveSystemModel = MVCUtil.GetModel<WaveSystemModel>(this);
    }

    protected override void OnViewDestroy()
    {
        StopCoroutine(_currentCoroutine);
    }

    #endregion

    public void StartWaveSystem(int startWave = 0)
    {
        SpawnWave(startWave);
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
        for (int i = 0; i < waveSection.Enemies.Length; i++)
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

            EnemyView enemyView = _enemyViewFactory.CreateEnemyView(waveSection.Enemies[i]);
            enemyView.transform.position = spawnPos;
        }

        if (waveSection.TimeToFightEnemiesInSeconds == WaveSystemModel.TIME_TO_FIGHT_UNTIL_ALL_EXTINCT)
        {
            // Wait for all the enemies to be killed or removed from play.
            _currentCoroutine = StartCoroutine(WaitForSectionExtiction(waveSection.Enemies, waveSectionIndex, endOfWaveSectionCallback));
        }
        else
        {
            // Wait for amount of time until next section is started.
            _currentCoroutine = StartCoroutine(WaitToEndSection(waveSection.TimeToFightEnemiesInSeconds, waveSectionIndex, endOfWaveSectionCallback));
        }
    }

    private IEnumerator WaitForSectionExtiction(EnemyModel[] sectionEnemies, int sectionIndex, Action<int> endCallback)
    {
        List<EnemyModel> trackingEnemies = new List<EnemyModel>(sectionEnemies);
        while(trackingEnemies.Count > 0)
        {
            for(int i = trackingEnemies.Count - 1; i >= 0; i--)
            {
                EnemyModel e = trackingEnemies[i];
                if(e.IsDead || e.IsDestroyed)
                {
                    trackingEnemies.RemoveAt(i);
                }
            }

            yield return null;
        }

        endCallback(sectionIndex);
    }

    private IEnumerator WaitToEndSection(float secondsToWait, int sectionIndex, Action<int> endCallback)
    {
        yield return new WaitForSeconds(secondsToWait);
        endCallback(sectionIndex);
    }
}
