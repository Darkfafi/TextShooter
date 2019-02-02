using System;
using System.Xml;

public static class EnemyDataParser 
{
	public const string NODE_ENEMY_DATA_ID = "id";
	public const string NODE_ENEMY_DATA_TYPE = "type";
	public const string NODE_ENEMY_DATA_WEAPON = "weapon";
	public const string NODE_ENEMY_DATA_BEHAVIOUR = "behaviour";
	public const string NODE_ENEMY_DATA_MOVEMENT_SPEED = "movementSpeed";
	public const string NODE_ENEMY_DATA_EXTRA_WORDS = "extraWords";

	public static EnemyData ParseXml(string xmlString)
	{
		XmlDocument xmlDoc = new XmlDocument();
		xmlDoc.LoadXml(xmlString);
		return ParseXml(xmlDoc.DocumentElement);
	}

	public static EnemyData ParseXml(XmlNode root)
	{
		string id = null;
		string type = null;
		WeaponData? weaponData = null;
		string behaviour = "default";
		int extraWords = 0;
		float movementSpeed = 2f;

		foreach(XmlNode node in root)
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
					weaponData = WeaponDataParser.ParseXml(node, true);
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
		EnemyData enemyData = new EnemyData(id, type, weaponData.Value, behaviour, movementSpeed, extraWords);
		Validate(enemyData, weaponData.HasValue);
		return enemyData;
	}

	public static void Validate(EnemyData enemyData, bool hasWeaponData)
	{
		if(string.IsNullOrEmpty(enemyData.DataID))
			throw new Exception("Enemy Data has no ID defined");

		if(string.IsNullOrEmpty(enemyData.EnemyType))
			throw new Exception("Enemy Data has no EnemyType defined");

		if(!hasWeaponData)
			throw new Exception("Enemy Data has no WeaponData defined");

		if(string.IsNullOrEmpty(enemyData.BehaviourType))
			throw new Exception("Enemy Data has no BehaviourType defined");
	}
}
