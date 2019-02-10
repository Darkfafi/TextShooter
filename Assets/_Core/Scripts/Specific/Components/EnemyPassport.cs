public class EnemyPassport : BaseModelComponent
{
	public EnemyData EnemyData
	{
		get; private set;
	}

	protected override void Added()
	{
		SetEnabledState(false);
	}

	public void SetupPassport(EnemyData enemyData)
	{
		EnemyData = enemyData;
		SetEnabledState(true);
	}
}
