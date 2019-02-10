using System;
using System.Collections.Generic;
using System.Xml;

public class MobsDataParser : BaseTimelineEventDataParser
{
	protected override BaseTimelineEventData ParseFromXmlSpecificDataNode(XmlNode xmlDataNode, out Type timelineEventType)
	{
		MobsTimelineEventData data = new MobsTimelineEventData();
		List<MobsTimelineEventData.SpawnData> spawnInstructions = new List<MobsTimelineEventData.SpawnData>();
		timelineEventType = typeof(MobsTimelineEvent);

		foreach(XmlNode node in xmlDataNode)
		{
			if(node.Name == TimelineSpecificGlobals.NODE_MOBS_EVENT_DATA_SPAWN)
			{
				string enemyID = null;
				int amount = 1;
				int delayAfterSpawn = 0;
				float timeBetween = 0.35f;
				CameraUtils.Side spawnSide = CameraUtils.Side.Any;

				foreach(XmlNode spawnNode in node)
				{
					switch(spawnNode.Name)
					{
						case TimelineSpecificGlobals.NODE_MOBS_EVENT_DATA_SPAWN_ENEMY_ID:
							enemyID = spawnNode.InnerText;
							break;
						case TimelineSpecificGlobals.NODE_MOBS_EVENT_DATA_SPAWN_ENEMY_AMOUNT:
							int.TryParse(spawnNode.InnerText, out amount);
							break;
						case TimelineSpecificGlobals.NODE_MOBS_EVENT_DATA_SPAWN_DELAY_AFTER_SPAWN:
							int.TryParse(spawnNode.InnerText, out delayAfterSpawn);
							break;
						case TimelineSpecificGlobals.NODE_MOBS_EVENT_DATA_SPAWN_TIME_BETWEEN:
							float.TryParse(spawnNode.InnerText, out timeBetween);
							break;
						case TimelineSpecificGlobals.NODE_MOBS_EVENT_DATA_SPAWN_SIDE:
							spawnSide = CameraUtils.ParseToCameraSide(spawnNode.InnerText, CameraUtils.Side.Any);
							break;
					}
				}

				if(enemyID == null)
					throw new Exception("No Enemy Type was defined for the spawn instruction");

				spawnInstructions.Add(new MobsTimelineEventData.SpawnData()
				{
					EnemyID = enemyID,
					Amount = amount,
					DelayAfterSpawnInSeconds = delayAfterSpawn,
					TimeBetweenInSeconds = timeBetween,
					SpawnSide = spawnSide
				});
			}
		}

		data.MobSpawnInstructions = spawnInstructions.ToArray();
		return data;
	}
}