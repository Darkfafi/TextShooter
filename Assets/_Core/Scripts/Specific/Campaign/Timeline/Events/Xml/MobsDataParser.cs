using System.Collections.Generic;
using System.Xml;

public class MobsDataParser : BaseTimelineEventDataParser
{
	protected override BaseTimelineEventData ParseFromXmlSpecificDataNode(XmlNode xmlDataNode, out System.Type timelineEventType)
	{
		MobsTimelineEventData data = new MobsTimelineEventData();
		List<MobsTimelineEventData.SpawnData> spawnInstructions = new List<MobsTimelineEventData.SpawnData>();
		timelineEventType = typeof(MobsTimelineEvent);

		foreach(XmlNode node in xmlDataNode)
		{
			if(node.Name == TimelineSpecificGlobals.NODE_MOBS_EVENT_DATA_SPAWN)
			{
				string enemyType = null;
				int amount = 1;
				int timeForEnemies = 0;
				float timeBetween = 0.35f;
				CameraUtils.Side spawnSide = CameraUtils.Side.Any;

				foreach(XmlNode spawnNode in node)
				{
					switch(spawnNode.Name)
					{
						case TimelineSpecificGlobals.NODE_MOBS_EVENT_DATA_SPAWN_ENEMY_TYPE:
							enemyType = spawnNode.InnerText;
							break;
						case TimelineSpecificGlobals.NODE_MOBS_EVENT_DATA_SPAWN_ENEMY_AMOUNT:
							int.TryParse(spawnNode.InnerText, out amount);
							break;
						case TimelineSpecificGlobals.NODE_MOBS_EVENT_DATA_SPAWN_TIME_FOR_ENEMIES:
							int.TryParse(spawnNode.InnerText, out timeForEnemies);
							break;
						case TimelineSpecificGlobals.NODE_MOBS_EVENT_DATA_SPAWN_TIME_BETWEEN:
							float.TryParse(spawnNode.InnerText, out timeBetween);
							break;
						case TimelineSpecificGlobals.NODE_MOBS_EVENT_DATA_SPAWN_SIDE:
							spawnSide = CameraUtils.ParseToCameraSide(spawnNode.InnerText, CameraUtils.Side.Any);
							break;
					}
				}

				spawnInstructions.Add(new MobsTimelineEventData.SpawnData()
				{
					EnemyType = enemyType,
					Amount = amount,
					TimeForEnemies = timeForEnemies,
					TimeBetweenInSeconds = timeBetween,
					SpawnSide = spawnSide
				});
			}
		}

		data.MobSpawnInstructions = spawnInstructions.ToArray();
		return data;
	}
}