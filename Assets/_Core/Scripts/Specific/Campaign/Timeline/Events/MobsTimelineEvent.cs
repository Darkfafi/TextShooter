using System;
using System.Collections.Generic;
using UnityEngine;

public class MobsTimelineEvent : BaseTimelineEvent<MobsTimelineEventData, GameModel>
{
	private Queue<MobsTimelineEventData.SpawnData> _spawnInstructions;
	private List<Spawner> _runningSpawners = new List<Spawner>();
	private int _totalEnemiesToSpawn;
	private int _totalSpawnTimeInSeconds;
	private float _waitTime;

	protected override void PreActivate(TimelineState<GameModel> timelineState, MobsTimelineEventData data)
	{
		// Reset Values
		_waitTime = 0f;
		_totalSpawnTimeInSeconds = 1;
		_totalEnemiesToSpawn = 0;

		// Setup Spawn Instructions Queue
		_spawnInstructions = new Queue<MobsTimelineEventData.SpawnData>();
		MobsTimelineEventData.SpawnData[] dataSpawnInstructions = data.MobSpawnInstructions;
		float longestSpawnTime = 0f;
		for(int i = 0; i < dataSpawnInstructions.Length; i++)
		{
			_spawnInstructions.Enqueue(dataSpawnInstructions[i]);
			_totalEnemiesToSpawn += dataSpawnInstructions[i].Amount;
			_totalSpawnTimeInSeconds += dataSpawnInstructions[i].DelayAfterSpawnInSeconds;
			float spawnTimeDuration = (dataSpawnInstructions[i].TimeBetweenInSeconds * dataSpawnInstructions[i].Amount) + _totalSpawnTimeInSeconds;
			if(longestSpawnTime < spawnTimeDuration)
				longestSpawnTime = spawnTimeDuration;
		}

		_totalSpawnTimeInSeconds = Mathf.CeilToInt(longestSpawnTime);
	}

	protected override void EventActivated()
	{
		if(_spawnInstructions.Count > 0)
		{
			Game.TimekeeperModel.ListenToFrameTick(EventTickUpdate);
		}
		else
		{
			EndEvent();
		}
	}

	private void EventTickUpdate(float deltaTime, float timeScale)
	{
		if(_waitTime > 0f)
		{
			_waitTime -= deltaTime * timeScale;
		}
		else if(_spawnInstructions.Count > 0)
		{
			MobsTimelineEventData.SpawnData instruction = _spawnInstructions.Dequeue();
			Spawner spawner = new Spawner(UniqueEventId, Game, instruction);
			_runningSpawners.Add(spawner);
			spawner.Execute(
				(e)=> 
				{
					e.Clean();
					_runningSpawners.Remove(e);
					if(_runningSpawners.Count == 0 && !HasEventEndingProgressors)
					{
						EndEvent();
					}
				}
			);

			_waitTime = instruction.DelayAfterSpawnInSeconds;
		}
	}

	protected override BaseTimelineEventProgressor CreateSupportedProgressor(string progressorName)
	{
		switch(progressorName)
		{
			case TimelineSpecificGlobals.PROGRESSOR_NAME_KILLS:
				return new KillsProgressor(UniqueEventId, _totalEnemiesToSpawn);
			case TimelineSpecificGlobals.PROGRESSOR_NAME_TIME:
				return new TimeProgressor(Game.TimekeeperModel, _totalSpawnTimeInSeconds);
			default:
				return null;
		}
	}

	protected override void EventDeactivated()
	{
		Game.TimekeeperModel.UnlistenFromFrameTick(EventTickUpdate);

		for(int i = _runningSpawners.Count - 1; i >= 0; i--)
		{
			_runningSpawners[i].Clean();
		}

		_runningSpawners.Clear();
	}

	protected override void ExecuteEndingTypeEffect(string endingType)
	{
		if(endingType == TimelineSpecificGlobals.CONST_MOBS_EVENT_DATA_ENDING_TYPE_DESTROY)
		{
			EntityModel[] enemies = EntityTracker.Instance.GetAll<EntityModel>(e => e.ModelTags.HasTag(UniqueEventId));

			for(int i = 0; i < enemies.Length; i++)
			{
				enemies[i].Destroy();
			}
		}
	}

	private class Spawner
	{
		private GameModel _game;
		private MobsTimelineEventData.SpawnData _mobsSpawnData;

		private bool _isRunning = false;
		private Action<Spawner> _spawnerEndedCallback;

		private float _timeTillNextSpawnInSeconds;
		private int _current = 0;
		private string _eventSpawnId;

		public Spawner(string eventSpawnId, GameModel game, MobsTimelineEventData.SpawnData spawnData)
		{
			_eventSpawnId = eventSpawnId;
			_game = game;
			_mobsSpawnData = spawnData;
		}

		public void Execute(Action<Spawner> onSpawnerEndedCallback)
		{
			if(_isRunning)
				return;

			_isRunning = true;

			_current = 0;
			_timeTillNextSpawnInSeconds = 0f;
			_spawnerEndedCallback = onSpawnerEndedCallback;
			_game.TimekeeperModel.ListenToFrameTick(OnTick);
		}

		public void End()
		{
			if(!_isRunning)
				return;

			_isRunning = false;
			_game.TimekeeperModel.UnlistenFromFrameTick(OnTick);

			if(_spawnerEndedCallback != null)
				_spawnerEndedCallback(this);
		}

		public void Clean()
		{
			End();
			_game = null;
		}

		private void OnTick(float deltaTime, float timeScale)
		{
			if(_timeTillNextSpawnInSeconds > 0f)
			{
				_timeTillNextSpawnInSeconds -= deltaTime * timeScale;
			}
			else
			{
				_timeTillNextSpawnInSeconds += _mobsSpawnData.TimeBetweenInSeconds;
				Spawn();
			}
		}

		private void Spawn()
		{
			if(_current < _mobsSpawnData.Amount)
			{
				EnemyModel enemy = _game.Factories.EnemyFactory.Create(new EnemyFactoryData(_mobsSpawnData.EnemyType));
				enemy.ModelTransform.Position = _game.GameCamera.GetOutOfMaxOrthographicLocation(_mobsSpawnData.SpawnSide);
				enemy.ModelTags.AddTag(_eventSpawnId);
				enemy.ModelTags.AddTag(Tags.TARGETABLE);
				_current++;

				if(_current >= _mobsSpawnData.Amount)
					End();
			}
			else
			{
				End();
			}
		}
	}
}

public class MobsTimelineEventData : BaseTimelineEventData
{
	public SpawnData[] MobSpawnInstructions;

	public struct SpawnData
	{
		public CameraUtils.Side SpawnSide;
		public string EnemyType;
		public int Amount;
		public int DelayAfterSpawnInSeconds;
		public float TimeBetweenInSeconds;
	}
}