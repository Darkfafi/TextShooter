using System.Collections.Generic;
using System.Xml;

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