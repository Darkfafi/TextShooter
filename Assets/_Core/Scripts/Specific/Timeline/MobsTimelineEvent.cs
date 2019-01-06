using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class MobsTimelineEvent : BaseTimelineEvent<MobsTimelineEventData, GameModel>
{
	private Queue<MobsSpawnData> _spawnInstructions;
	private int _totalEnemiesToSpawn;
	private int _totalSpawnTimeInSeconds;
	private float _waitTime;

	protected override void PreActivate(TimelineState<GameModel> timelineState, MobsTimelineEventData data)
	{
		// Reset Values
		_waitTime = 0f;
		_totalSpawnTimeInSeconds = 0;
		_totalEnemiesToSpawn = 0;

		// Setup Spawn Instructions Queue
		_spawnInstructions = new Queue<MobsSpawnData>();
		MobsSpawnData[] dataSpawnInstructions = data.MobSpawnInstructions;
		for(int i = 0; i < dataSpawnInstructions.Length; i++)
		{
			_spawnInstructions.Enqueue(dataSpawnInstructions[i]);
			_totalEnemiesToSpawn += dataSpawnInstructions[i].Amount;
			_totalSpawnTimeInSeconds += dataSpawnInstructions[i].TimeForEnemies;
		}
	}

	protected override void EventActivated()
	{
		Game.TimekeeperModel.ListenToFrameTick(EventTickUpdate);
	}

	private void EventTickUpdate(float deltaTime, float timeScale)
	{
		if(_waitTime >= 0f)
		{
			_waitTime -= deltaTime * timeScale;
		}

		if(_spawnInstructions.Count > 0 && _waitTime <= 0f)
		{
			MobsSpawnData instruction = _spawnInstructions.Dequeue();
			instruction.SpawnEnemies(UniqueEventId, Game);
			_waitTime = instruction.TimeForEnemies;
		}
	}

	protected override BaseTimelineEventProgressor[] SetupProgressorsSupported()
	{
		return new BaseTimelineEventProgressor[]
		{
			new KillsProgressor(UniqueEventId, _totalEnemiesToSpawn),
			new TimeProgressor(Game.TimekeeperModel, _totalSpawnTimeInSeconds)
		};
	}

	protected override void EventDeactivated()
	{
		Game.TimekeeperModel.UnlistenFromFrameTick(EventTickUpdate);
	}
}

public class MobsTimelineEventData : BaseTimelineEventData
{
	public MobsSpawnData[] MobSpawnInstructions;
}

public struct MobsSpawnData
{
	public string EnemyType;
	public int Amount;
	public int TimeForEnemies;

	public void SpawnEnemies(string eventSpawnId, GameModel game, float spawnMargin = 1f)
	{
		CameraModel gameCamera = game.GameCamera;
		float spawnDistY = game.GameCamera.MaxOrtographicSize + spawnMargin;
		float spawnDistX = spawnDistY * Screen.width / Screen.height;

		for(int i = 0; i < Amount; i++)
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

			EnemyModel enemy = EnemyFactory.CreateEnemy(game.TimekeeperModel, EnemyType);
			enemy.ModelTransform.Position = spawnPos;
			enemy.ModelTags.AddTag(eventSpawnId);
		}
		
	}
}

public class MobsDataParser : BaseTimelineEventDataParser
{
	protected override BaseTimelineEventData ParseFromXmlSpecificDataNode(XmlNode xmlDataNode, out System.Type timelineEventType)
	{
		MobsTimelineEventData data = new MobsTimelineEventData();
		List<MobsSpawnData> spawnInstructions = new List<MobsSpawnData>();
		timelineEventType = typeof(MobsTimelineEvent);

		foreach(XmlNode node in xmlDataNode)
		{
			if(node.Name == TimelineSpecificGlobals.NODE_MOBS_EVENT_DATA_SPAWN)
			{
				string enemyType = null;
				int amount = 1;
				int timeForEnemies = 0;
				foreach(XmlNode spawnNode in node)
				{
					switch(spawnNode.Name)
					{
						case TimelineSpecificGlobals.NODE_MOBS_EVENT_DATA_SPAWN_ENEMY_TYPE:
							enemyType = spawnNode.InnerText;
							break;
						case TimelineSpecificGlobals.NODE_MOBS_EVENT_DATA_SPAWN_ENEMY_AMOUNT:
							amount = int.Parse(spawnNode.InnerText);
							break;
						case TimelineSpecificGlobals.NODE_MOBS_EVENT_DATA_SPAWN_TIME_FOR_ENEMIES:
							timeForEnemies = int.Parse(spawnNode.InnerText);
							break;
					}
				}

				spawnInstructions.Add(new MobsSpawnData()
				{
					EnemyType = enemyType,
					Amount = amount,
					TimeForEnemies = timeForEnemies
				});
			}
		}

		data.MobSpawnInstructions = spawnInstructions.ToArray();
		return data;
	}
}