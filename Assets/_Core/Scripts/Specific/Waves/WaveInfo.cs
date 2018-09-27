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
