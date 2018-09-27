using System;
using UnityEngine;

public class WaveSystemModel : BaseModel
{
    public const float TIME_TO_FIGHT_UNTIL_ALL_EXTINCT = -1f;

    public delegate void WaveHandler(WaveInfo waveInfoObject);
    public event WaveHandler SpawnWaveEvent;

    public event Action<EnemyModel> SpawnEnemyEvent;

    public int CurrentWave { get; private set; }
    public float SpawnDistanceY { get; private set; }
    public float SpawnDistanceX { get; private set; }

    // Global
    private Tracker _sectionTracker;
    private int _currentSection = 0;
    private WaveInfo _currentWaveInfo;

    public WaveSystemModel(TimekeeperModel timekeeper)
    {
        _sectionTracker = new Tracker(timekeeper);
    }

    public void StartWaveSystem(int startWave = 0)
    {
        InternalStartWave(startWave);
    }

    protected override void OnModelDestroy()
    {
        base.OnModelDestroy();
        _sectionTracker.Clean();
        _sectionTracker = null;
        _currentWaveInfo = null;
    }

    public void SetSpawnDistance(float x, float y)
    {
        SpawnDistanceX = x;
        SpawnDistanceY = y;
    }

    private void SpawnWaveSection(WaveSectionObject waveSection)
    {
        EnemyModel[] enemiesSpawned = new EnemyModel[waveSection.Enemies.Length];

        for (int i = 0; i < waveSection.Enemies.Length; i++)
        {
            float distanceVarienceValue = UnityEngine.Random.value * 2f;
            bool fullX = UnityEngine.Random.value > 0.5f;
            int xMult = UnityEngine.Random.value > 0.5f ? 1 : -1;
            int yMult = UnityEngine.Random.value > 0.5f ? 1 : -1;
            float x = ((fullX) ? 1 : UnityEngine.Random.value);
            float y = ((!fullX) ? 1 : UnityEngine.Random.value);
            x = (Mathf.Lerp(0, SpawnDistanceX, x) + distanceVarienceValue) * xMult;
            y = (Mathf.Lerp(0, SpawnDistanceY, y) + distanceVarienceValue) * yMult;
            Vector2 spawnPos = new Vector2(x, y);

            EnemyModel enemy = waveSection.Enemies[i].CreateEnemy();
            enemy.Transform.Position = spawnPos;

            enemiesSpawned[i] = enemy;

            if (SpawnEnemyEvent != null)
            {
                SpawnEnemyEvent(enemy);
            }
        }

        if (waveSection.TimeToFightEnemiesInSeconds == TIME_TO_FIGHT_UNTIL_ALL_EXTINCT)
        {
            _sectionTracker.TrackExtinction(enemiesSpawned, OnEndSection);
        }
        else
        {
            _sectionTracker.TrackEndOfTime(waveSection.TimeToFightEnemiesInSeconds, OnEndSection);
        }
    }

    private void OnEndSection()
    {
        if(_currentWaveInfo == null)
        {
            return;
        }

        _currentSection++;
        if (_currentSection >= _currentWaveInfo.WaveSections.Length)
        {
            // End Wave
            Debug.Log("TODO: END WAVE, NEXT WAVE LOGICS HERE");
            _currentSection = 0;
        }
        else
        {
            SpawnWaveSection(_currentWaveInfo.WaveSections[_currentSection]);
        }
    }

    private void InternalStartWave(int wave)
    {
        _currentWaveInfo = GetWaveInfo(wave);
        _currentSection = 0;

        SpawnWaveSection(_currentWaveInfo.WaveSections[_currentSection]);
    }

    private WaveInfo GetWaveInfo(int wave)
    {
        CurrentWave = wave;

        // TODO: Replace Test wave with real algorithm 
        WaveSectionObject section = new WaveSectionObject();
        section.Enemies = new EnemySpawnData[]
        {
            new EnemySpawnData(EnemyModel.EnemyCharacterType.Normal, 1, "First"),
            new EnemySpawnData(EnemyModel.EnemyCharacterType.Double, 1, "Second"),
            new EnemySpawnData(EnemyModel.EnemyCharacterType.Normal, 2, "The Number after two"),
        };

        section.TimeToFightEnemiesInSeconds = TIME_TO_FIGHT_UNTIL_ALL_EXTINCT;
        // -- End Test Wave

        WaveInfo waveInfo = new WaveInfo(wave, section);

        if (SpawnWaveEvent != null)
        {
            SpawnWaveEvent(waveInfo);
        }

        return waveInfo;
    }
}
