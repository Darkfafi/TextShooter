public class WaveSystemModel : EntityModel
{
    public const float TIME_TO_FIGHT_UNTIL_ALL_EXTINCT = -1f;

    public delegate void WaveHandler(WaveInfo waveInfoObject);
    public event WaveHandler SpawnWaveEvent;

    public int CurrentWave { get; private set; }

    public WaveInfo SpawnWave(int wave)
    {
        CurrentWave = wave;

        // TODO: Replace Test wave with real algorithm 
        WaveSectionObject section = new WaveSectionObject();
        section.Enemies = new EnemyModel[]
        {
            new EnemyModel(EnemyModel.EnemyCharacterType.Normal, 1, "First"),
            new EnemyModel(EnemyModel.EnemyCharacterType.Double, 1, "Second"),
            new EnemyModel(EnemyModel.EnemyCharacterType.Normal, 2, "The Number after two"),
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
        public EnemyModel[] Enemies;
        public float TimeToFightEnemiesInSeconds;
    }
}
