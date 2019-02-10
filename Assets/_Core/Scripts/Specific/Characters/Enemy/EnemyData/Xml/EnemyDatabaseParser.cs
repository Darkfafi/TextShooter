using System;
using System.Collections.Generic;
using System.Xml;

public static class EnemyDatabaseParser
{
	public const string NODE_ENEMY_DATA = "enemyData";

	public static StaticDatabase<EnemyData> ParseXml(string xmlString)
	{
		XmlDocument xmlDoc = new XmlDocument();
		xmlDoc.LoadXml(xmlString);
		return ParseXml(xmlDoc.DocumentElement);
	}

	public static StaticDatabase<EnemyData> ParseXml(XmlNode root)
	{
		List<EnemyData> data = new List<EnemyData>();

		foreach(XmlNode node in root)
		{
			if(node.Name == NODE_ENEMY_DATA)
			{
				data.Add(EnemyDataParser.ParseXml(node));
			}
		}

		return new StaticDatabase<EnemyData>(data.ToArray());
	}
}