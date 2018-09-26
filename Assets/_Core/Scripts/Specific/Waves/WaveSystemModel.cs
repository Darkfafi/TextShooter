using System;
using System.Collections.Generic;
using UnityEngine;

public class WaveSystemModel : BaseModel
{
    public enum TrackingType
    {
        None,
        WaitForTime,
        WaitForExtinction
    }

    public const float TIME_TO_FIGHT_UNTIL_ALL_EXTINCT = -1f;

    public delegate void WaveHandler(WaveInfo waveInfoObject);
    public event WaveHandler SpawnWaveEvent;

    public event Action<EnemyModel> SpawnEnemyEvent;

    public int CurrentWave { get; private set; }
    public float SpawnDistanceY { get; private set; }
    public float SpawnDistanceX { get; private set; }

    // Global
    private int _currentSection = 0;
    private float _timeInSection = 0f;
    private TimekeeperModel _timekeeperModel;

    // Tracking
    private TrackingType _currentTrackingType = TrackingType.None;
    private float _waitTimeInSeconds = 0f;
    private Action<int> _endOfSectionCallback;
    private List<EnemyModel> _trackingEnemies = new List<EnemyModel>();

    public WaveSystemModel(TimekeeperModel timekeeper)
    {
        _timekeeperModel = timekeeper;
        _timekeeperModel.ListenToFrameTick(Update);
    }

    protected override void OnModelDestroy()
    {
        base.OnModelDestroy();
        _timekeeperModel.UnlistenFromFrameTick(Update);
        _timekeeperModel = null;
    }

    public void StartWaveSystem(int startWave = 0)
    {
        WaveSectionLoop(GetWaveInfo(startWave), -1);
    }

    private void SpawnNextWave()
    {
        WaveSectionLoop(GetWaveInfo(CurrentWave + 1), -1);
    }

    public void SetSpawnDistance(float x, float y)
    {
        SpawnDistanceX = x;
        SpawnDistanceY = y;
    }

    private void WaveSectionLoop(WaveInfo waveInfo, int currentWaveSectionIndex)
    {
        _currentSection = currentWaveSectionIndex + 1;
        _timeInSection = 0;
        if (_currentSection >= waveInfo.WaveSections.Length)
        {
            // End Wave
            Debug.Log("TODO: END WAVE, NEXT WAVE LOGICS HERE");
            _currentSection = 0;
        }
        else
        {
            SpawnWaveSection(waveInfo.WaveSections[_currentSection], (index) =>
            {
                WaveSectionLoop(waveInfo, index);
            });
        }
    }

    private void SpawnWaveSection(WaveSectionObject waveSection, Action<int> endOfWaveSectionCallback)
    {
        _endOfSectionCallback = endOfWaveSectionCallback;

        if (waveSection.TimeToFightEnemiesInSeconds == TIME_TO_FIGHT_UNTIL_ALL_EXTINCT)
        {
            _currentTrackingType = TrackingType.WaitForExtinction;
        }
        else
        {
            _currentTrackingType = TrackingType.WaitForTime;
            _waitTimeInSeconds = waveSection.TimeToFightEnemiesInSeconds;
        }

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

            if(_currentTrackingType == TrackingType.WaitForExtinction)
            {
                _trackingEnemies.Add(enemy);
                enemy.DestroyEvent += OnDestroyEvent;
                enemy.DeathEvent += OnDeathEvent;
            }

            if(SpawnEnemyEvent != null)
            {
                SpawnEnemyEvent(enemy);
            }
        }
    }

    private void Update(float deltaTime, float timeScale)
    {
        _timeInSection += deltaTime * timeScale;
        if (_currentTrackingType == TrackingType.WaitForTime)
        {
            if(_timeInSection > _waitTimeInSeconds)
            {
                _endOfSectionCallback(_currentSection);
            }
        }
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

    private void OnDestroyEvent(BaseModel enemy)
    {
        OnDeathEvent((EnemyModel)enemy);
    }

    private void OnDeathEvent(EnemyModel enemy)
    {
        enemy.DestroyEvent -= OnDestroyEvent;
        enemy.DeathEvent -= OnDeathEvent;

        _trackingEnemies.Remove(enemy);
        if (_trackingEnemies.Count == 0)
        {
            _endOfSectionCallback(_currentSection);
        }
    }

    public class WaveInfo
    {
        public int WaveIndex { get; private set; }
        public WaveSectionObject[] WaveSections { get; private set; }

        public WaveInfo(int waveIndex, params WaveSectionObject[] waveSections)
        {
            WaveIndex = waveIndex;
            WaveSections = waveSections;
        }
    }

    public struct WaveSectionObject
    {
        public EnemySpawnData[] Enemies;
        public float TimeToFightEnemiesInSeconds;
    }

    public struct EnemySpawnData
    {
        public string EnemyWord { get; private set; }
        public string[] EnemyNextWords { get; private set; }
        public EnemyModel.EnemyCharacterType EnemyCharacterType { get; private set; }
        public int Damage { get; private set; }

        public EnemySpawnData(EnemyModel.EnemyCharacterType enemyType, int damage, string word, params string[] nextWords)
        {
            EnemyWord = word;
            EnemyNextWords = nextWords;
            EnemyCharacterType = enemyType;
            Damage = damage;
        }

        public EnemyModel CreateEnemy()
        {
            return new EnemyModel(EnemyCharacterType, Damage, EnemyWord, EnemyNextWords);
        }
    }
}
