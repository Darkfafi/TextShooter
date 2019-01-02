using System.Collections.Generic;
using UnityEngine;

public class MobsTimelineEvent : TimelineEvent<MobTimelineEventData>
{
	public override int EventType
	{
		get
		{
			return TimelineEventGlobals.PROGRESS_TYPE_KILLS;
		}
	}

	private TimelineEventProgressor _timeProgressor;
	private TimelineEventProgressor _killsProgressor;

	private List<TimelineEventProgressor> _progressorInUse;
	private Queue<SpawnData> _spawnInstructions;
	private float _secondCounter = 0f;
	private float _waitTime = 0f;
	private int _enemiesSpawned = 0;

	private float _orthographicSpawnMargin = 1f;
	private EntityFilter<CameraModel> _gameCameraFilter;
	private EntityFilter<EnemyModel> _spawnedEnemyTrackerFilter;

	private string _mobTimelineEventSpawnId;

	public override TimelineEventProgressor[] GetProgressors()
	{
		return _progressorInUse.ToArray();
	}

	protected override void EventActivated()
	{
		// Reset Values
		_waitTime = 0;
		_secondCounter = 0;
		_enemiesSpawned = 0;
		_progressorInUse = new List<TimelineEventProgressor>();

		// Setup Filters
		_gameCameraFilter = EntityFilter<CameraModel>.Create();

		_mobTimelineEventSpawnId = string.Concat(GetType().FullName, GetHashCode().ToString(), Random.Range(0, 100));
		_spawnedEnemyTrackerFilter = EntityFilter<EnemyModel>.Create(FilterRules.CreateHasAnyTagsFilter(_mobTimelineEventSpawnId));
		_spawnedEnemyTrackerFilter.TrackedEvent += OnTrackedEvent;
		_spawnedEnemyTrackerFilter.UntrackedEvent += OnUntrackedEvent;

		// Setup Spawn Instructions Queue
		int enemyAmount = 0;
		_spawnInstructions = new Queue<SpawnData>();
		SpawnData[] spawnInstructionsParameter = EventParams.TimelineEventData.MobSpawnInstructions;
		for(int i = 0; i < spawnInstructionsParameter.Length; i++)
		{
			_spawnInstructions.Enqueue(spawnInstructionsParameter[i]);
			enemyAmount += spawnInstructionsParameter[i].Amount;
		}

		// Setup Progressors
		if(EventParams.TimelineEventData.UseKillsProgressor)
		{
			_progressorInUse.Add(_killsProgressor = new TimelineEventProgressor(enemyAmount));
		}

		if(EventParams.TimelineEventData.TimeForMobsInSeconds > 0)
		{
			_progressorInUse.Add(_timeProgressor = new TimelineEventProgressor(EventParams.TimelineEventData.TimeForMobsInSeconds));
		}
	}

	protected override void EventTickUpdate(float deltaTime, float timeScale)
	{
		if(_waitTime >= 0f)
		{
			_waitTime -= deltaTime * timeScale;
		}

		if(_spawnInstructions.Count > 0 && _waitTime <= 0f)
		{
			SpawnData instruction = _spawnInstructions.Dequeue();
			Spawn(instruction);
			_waitTime = instruction.PauseInSeconds;

			if(_spawnInstructions.Count == 0 && _progressorInUse.Count == 0)
			{
				EndEvent(true);
			}
		}

		if(EventParams.TimelineEventData.TimeForMobsInSeconds > 0)
		{
			_secondCounter += deltaTime * timeScale;
			if(_secondCounter >= 1f)
			{
				_secondCounter -= 1f;
				_timeProgressor.UpdateValue(_timeProgressor.CurrentValue + 1);
				if(_timeProgressor.IsGoalMatched)
				{
					EndEvent(true);
				}
			}
		}
	}

	protected override void EventDeactivated()
	{
		EnemyModel[] enemiesTracking = _spawnedEnemyTrackerFilter.GetAll();

		for(int i = 0; i < enemiesTracking.Length; i++)
		{
			enemiesTracking[i].ModelTags.RemoveTag(_mobTimelineEventSpawnId);
		}

		_spawnedEnemyTrackerFilter.TrackedEvent -= OnTrackedEvent;
		_spawnedEnemyTrackerFilter.UntrackedEvent -= OnUntrackedEvent;
		_spawnedEnemyTrackerFilter.Clean();
		_spawnedEnemyTrackerFilter = null;
		_gameCameraFilter.Clean();
		_gameCameraFilter = null;
		_progressorInUse.Clear();
		_progressorInUse = null;
		_timeProgressor = null;
		_killsProgressor = null;
	}

	private void Spawn(SpawnData spawnData)
	{
		CameraModel gameCamera = _gameCameraFilter.GetFirst((e) => !e.IsDestroyed);
		if(gameCamera != null)
		{
			float spawnDistY = gameCamera.MaxOrtographicSize + _orthographicSpawnMargin;
			float spawnDistX = spawnDistY * Screen.width / Screen.height;

			for(int i = 0; i < spawnData.Amount; i++)
			{
				float distanceVarienceValue = Random.value * 2f;
				bool fullX = Random.value > 0.5f;
				int xMult = Random.value > 0.5f ? 1 : -1;
				int yMult = Random.value > 0.5f ? 1 : -1;
				float x = ((fullX) ? 1 : Random.value);
				float y = ((!fullX) ? 1 : Random.value);
				x = (Mathf.Lerp(0, spawnDistX, x) + distanceVarienceValue) * xMult;
				y = (Mathf.Lerp(0, spawnDistY, y) + distanceVarienceValue) * yMult;
				Vector2 spawnPos = new Vector2(x, y);

				EnemyModel enemy = EnemyFactory.CreateEnemy(timekeeperModel, spawnData.EnemyType);
				_enemiesSpawned++;
				enemy.ModelTransform.Position = spawnPos;
				enemy.ModelTags.AddTag(_mobTimelineEventSpawnId);
			}
		}
	}

	private void OnTrackedEvent(EnemyModel enemy)
	{
		if(EventParams.TimelineEventData.UseKillsProgressor)
		{
			enemy.DeathEvent += OnDeathEvent;
		}
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
		int enemyAmountToGo = (_killsProgressor.GoalValue - _enemiesSpawned);
		_killsProgressor.UpdateValue(_killsProgressor.GoalValue - (_spawnedEnemyTrackerFilter.GetAll((e) => !e.IsDead).Length + enemyAmountToGo));
		if(_killsProgressor.IsGoalMatched)
		{
			EndEvent(true);
		}
	}
}


public struct MobTimelineEventData : ITimelineEventData
{
	public int TimeForMobsInSeconds;
	public bool UseKillsProgressor;
	public SpawnData[] MobSpawnInstructions;
}

public struct SpawnData
{
	public string EnemyType;
	public int Amount;
	public float PauseInSeconds;
}