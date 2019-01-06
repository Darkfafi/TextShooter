﻿using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class MobsTimelineEvent : BaseTimelineEvent<MobsTimelineEventData, GameModel>
{
	private Queue<SpawnData> _spawnInstructions;
	private string _mobTimelineEventSpawnId;
	private int _totalEnemiesToSpawn;
	private int _totalSpawnTimeInSeconds;
	private float _waitTime;

	protected override void PreActivate(TimelineState<GameModel> timelineState, MobsTimelineEventData data)
	{
		// Reset Values
		_waitTime = 0f;
		_totalSpawnTimeInSeconds = 0;
		_totalEnemiesToSpawn = 0;

		_mobTimelineEventSpawnId = string.Concat(GetType().FullName, GetHashCode().ToString(), Random.Range(0, 100));

		// Setup Spawn Instructions Queue
		_spawnInstructions = new Queue<SpawnData>();
		SpawnData[] dataSpawnInstructions = data.MobSpawnInstructions;
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
			SpawnData instruction = _spawnInstructions.Dequeue();
			instruction.SpawnEnemies(_mobTimelineEventSpawnId, Game);
			_waitTime = instruction.TimeForEnemies;
		}
	}

	protected override BaseTimelineEventProgressor[] SetupProgressors(TimelineState<GameModel> timelineState, MobsTimelineEventData data)
	{
		List<BaseTimelineEventProgressor> progressors = new List<BaseTimelineEventProgressor>();
		if(EventData.UseKillsProgressor)
		{
			progressors.Add(new KillsProgressor(TimelineSpecificGlobals.CONST_MOBS_EVENT_DATA_PROGRESSOR_KILLS, _mobTimelineEventSpawnId, _totalEnemiesToSpawn));
		}

		if(EventData.TimeForMobsInSeconds > 0 || progressors.Count == 0)
		{
			progressors.Add(new TimeProgressor(TimelineSpecificGlobals.CONST_MOBS_EVENT_DATA_PROGRESSOR_TIME, Game.TimekeeperModel, _totalSpawnTimeInSeconds + EventData.TimeForMobsInSeconds));
		}
		

		return progressors.ToArray();
	}

	protected override void EventDeactivated()
	{
		Game.TimekeeperModel.UnlistenFromFrameTick(EventTickUpdate);
	}
}

public class MobsTimelineEventData : BaseTimelineEventData
{
	public int TimeForMobsInSeconds;
	public bool UseKillsProgressor;
	public SpawnData[] MobSpawnInstructions;
}

public struct SpawnData
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
		List<SpawnData> spawnInstructions = new List<SpawnData>();
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
				spawnInstructions.Add(new SpawnData()
				{
					EnemyType = enemyType,
					Amount = amount,
					TimeForEnemies = timeForEnemies
				});
			}
			else if(node.Name == TimelineSpecificGlobals.NODE_MOBS_EVENT_DATA_PROGRESSOR)
			{
				switch(node.InnerText)
				{
					case TimelineSpecificGlobals.CONST_MOBS_EVENT_DATA_PROGRESSOR_TIME:
						string valueText = node.Attributes[TimelineSpecificGlobals.ATTRIBUTE_MOBS_EVENT_DATA_PROGRESSOR_VALUE] == null ? "0" : node.Attributes[TimelineSpecificGlobals.ATTRIBUTE_MOBS_EVENT_DATA_PROGRESSOR_VALUE].InnerText;
						data.TimeForMobsInSeconds = int.Parse(valueText);
						break;
					case TimelineSpecificGlobals.CONST_MOBS_EVENT_DATA_PROGRESSOR_KILLS:
						data.UseKillsProgressor = true;
						break;
				}
			}
		}

		data.MobSpawnInstructions = spawnInstructions.ToArray();
		return data;
	}
}