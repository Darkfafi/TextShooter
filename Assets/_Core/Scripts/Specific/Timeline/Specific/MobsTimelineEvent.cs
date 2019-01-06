using System.Collections.Generic;
using UnityEngine;

public class MobsTimelineEvent : BaseTimelineEvent<MobTimelineEventData, GameModel>
{
	private Queue<SpawnData> _spawnInstructions;
	private float _waitTime = 0f;

	private float _orthographicSpawnMargin = 1f;
	private EntityFilter<CameraModel> _gameCameraFilter;

	private string _mobTimelineEventSpawnId;
	private int _totalEnemiesToSpawn;

	protected override void PreActivate(TimelineState<GameModel> timelineState, MobTimelineEventData data)
	{
		// Reset Values
		_waitTime = 0;
		_totalEnemiesToSpawn = 0;

		// Setup Filters
		_gameCameraFilter = EntityFilter<CameraModel>.Create();

		_mobTimelineEventSpawnId = string.Concat(GetType().FullName, GetHashCode().ToString(), Random.Range(0, 100));

		// Setup Spawn Instructions Queue
		_spawnInstructions = new Queue<SpawnData>();
		SpawnData[] spawnInstructionsParameter = data.MobSpawnInstructions;
		for(int i = 0; i < spawnInstructionsParameter.Length; i++)
		{
			_spawnInstructions.Enqueue(spawnInstructionsParameter[i]);
			_totalEnemiesToSpawn += spawnInstructionsParameter[i].Amount;
		}
	}

	protected override void EventActivated()
	{
		Game.TimekeeperModel.ListenToFrameTick(EventTickUpdate);
	}

	private void OnGoalMatchedEvent(BaseTimelineEventProgressor progressor)
	{
		EndEvent();
	}

	private void EventTickUpdate(float deltaTime, float timeScale)
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

			if(_spawnInstructions.Count == 0 && ProgressorsInUse == 0)
			{
				EndEvent();
			}
		}
	}

	protected override BaseTimelineEventProgressor[] SetupProgressors(TimelineState<GameModel> timelineState, MobTimelineEventData data)
	{
		List<BaseTimelineEventProgressor> progressors = new List<BaseTimelineEventProgressor>();
		if(EventData.UseKillsProgressor)
		{
			progressors.Add(new MobsKillsProgressor(_mobTimelineEventSpawnId, _totalEnemiesToSpawn));
		}

		if(EventData.TimeForMobsInSeconds > 0)
		{
			progressors.Add(new TimeProgressor(Game.TimekeeperModel, EventData.TimeForMobsInSeconds));
		}

		return progressors.ToArray();
	}

	protected override void EventDeactivated()
	{
		Game.TimekeeperModel.UnlistenFromFrameTick(EventTickUpdate);

		_gameCameraFilter.Clean();
		_gameCameraFilter = null;
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

				EnemyModel enemy = EnemyFactory.CreateEnemy(Game.TimekeeperModel, spawnData.EnemyType);
				enemy.ModelTransform.Position = spawnPos;
				enemy.ModelTags.AddTag(_mobTimelineEventSpawnId);
			}
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