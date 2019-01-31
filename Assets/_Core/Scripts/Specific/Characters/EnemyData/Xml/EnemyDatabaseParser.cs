using System;
using System.Collections.Generic;
using System.Xml;

public static class EnemyDatabaseParser
{
	public const string NODE_ENEMY_DATA = "enemyData";
	public const string NODE_ENEMY_DATA_ID = "id";
	public const string NODE_ENEMY_DATA_TYPE = "type";
	public const string NODE_ENEMY_DATA_WEAPON = "weapon";
	public const string NODE_ENEMY_DATA_BEHAVIOUR = "behaviour";
	public const string NODE_ENEMY_DATA_MOVEMENT_SPEED = "movementSpeed";
	public const string NODE_ENEMY_DATA_EXTRA_WORDS = "extraWords";

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
				try
				{
					data.Add(ParseEnemyDataNode(node));
				}
				catch(Exception e)
				{
					UnityEngine.Debug.LogError("Could not parse EnemyDatabase XML. Error: " + e.Message);
				}
			}
		}

		return new StaticDatabase<EnemyData>(data.ToArray());
	}

	private static EnemyData ParseEnemyDataNode(XmlNode dataNode)
	{
		string id = null;
		string type = null;
		string weapon = null;
		string behaviour = "default";
		int extraWords = 0;
		float movementSpeed = 2f;

		foreach(XmlNode node in dataNode)
		{
			switch(node.Name)
			{
				case NODE_ENEMY_DATA_ID:
					id = node.InnerText;
					break;
				case NODE_ENEMY_DATA_TYPE:
					type = node.InnerText;
					break;
				case NODE_ENEMY_DATA_WEAPON:
					weapon = node.InnerText;
					break;
				case NODE_ENEMY_DATA_BEHAVIOUR:
					behaviour = node.InnerText;
					break;
				case NODE_ENEMY_DATA_EXTRA_WORDS:
					int.TryParse(node.InnerText, out extraWords);
					break;
				case NODE_ENEMY_DATA_MOVEMENT_SPEED:
					float.TryParse(node.InnerText, out movementSpeed);
					break;
			}
		}

		EnemyData enemyData = new EnemyData(id, type, weapon, behaviour, movementSpeed, extraWords);
		Validate(enemyData);
		return enemyData;
	}

	private static void Validate(EnemyData enemyData)
	{
		if(string.IsNullOrEmpty(enemyData.DataID))
			throw new Exception("Enemy Data has no ID defined");

		if(string.IsNullOrEmpty(enemyData.EnemyType))
			throw new Exception("Enemy Data has no EnemyType defined");

		if(string.IsNullOrEmpty(enemyData.WeaponType))
			throw new Exception("Enemy Data has no WeaponType defined");

		if(string.IsNullOrEmpty(enemyData.BehaviourType))
			throw new Exception("Enemy Data has no BehaviourType defined");
	}
}