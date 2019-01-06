public class KillsProgressor : BaseTimelineEventProgressor
{
	private EntityFilter<EnemyModel> _spawnedEnemyTrackerFilter;
	private string _eventEnemyTag;
	private int _enemiesSpawned;

	public KillsProgressor(string eventEnemyTag, int goalKills) : base(goalKills)
	{
		_eventEnemyTag = eventEnemyTag;
	}

	public override void StartProgressor()
	{
		_enemiesSpawned = 0;
		_spawnedEnemyTrackerFilter = EntityFilter<EnemyModel>.Create(FilterRules.CreateHasAnyTagsFilter(_eventEnemyTag));
		_spawnedEnemyTrackerFilter.TrackedEvent += OnTrackedEvent;
		_spawnedEnemyTrackerFilter.UntrackedEvent += OnUntrackedEvent;
	}

	public override void EndProgressor()
	{
		EnemyModel[] spawnedEnemies = _spawnedEnemyTrackerFilter.GetAll(e => !e.IsDead);

		for(int i = 0; i < spawnedEnemies.Length; i++)
		{
			spawnedEnemies[i].DeathEvent -= OnDeathEvent;
		}

		_spawnedEnemyTrackerFilter.TrackedEvent -= OnTrackedEvent;
		_spawnedEnemyTrackerFilter.UntrackedEvent -= OnUntrackedEvent;
		_spawnedEnemyTrackerFilter.Clean();
		_spawnedEnemyTrackerFilter = null;
	}

	private void OnTrackedEvent(EnemyModel enemy)
	{
		enemy.DeathEvent += OnDeathEvent;
		_enemiesSpawned++;
	}

	private void OnUntrackedEvent(EnemyModel enemy)
	{
		enemy.DeathEvent -= OnDeathEvent;
		if(!enemy.IsDead)
		{
			OnDeathEvent(enemy);
		}
	}

	private void OnDeathEvent(EnemyModel enemy)
	{
		enemy.DeathEvent -= OnDeathEvent;
		int enemyAmountToGo = (GoalValue - _enemiesSpawned);
		UpdateValue(GoalValue - (_spawnedEnemyTrackerFilter.GetAll((e) => !e.IsDead).Length + enemyAmountToGo));
	}
}
