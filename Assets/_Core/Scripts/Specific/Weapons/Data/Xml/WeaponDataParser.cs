using System;
using System.Xml;

public static class WeaponDataParser
{
	public const string NODE_WEAPON_DATA_ID = "id";
	public const string NODE_WEAPON_DATA_DAMAGE = "damage";
	public const string CONST_WEAPON_DATA_DAMAGE_KILL = "kill";

	public const string NODE_WEAPON_DATA_RADIUS = "radius";
	public const string NODE_WEAPON_DATA_COOLDOWN = "cooldown";

	public static WeaponData ParseXml(string xmlString, bool allowedOnlyID)
	{
		XmlDocument xmlDoc = new XmlDocument();
		xmlDoc.LoadXml(xmlString);
		return ParseXml(xmlDoc.DocumentElement, allowedOnlyID);
	}

	public static WeaponData ParseXml(XmlNode root, bool idWeaponDataAllowed)
	{
		WeaponData weaponData;

		if(root.ChildNodes.Count > 1)
		{
			string id = null;
			int damage = 1;
			float radius = 1f;
			float cooldown = -1f;

			foreach(XmlNode node in root)
			{
				switch(node.Name)
				{
					case NODE_WEAPON_DATA_ID:
						id = node.InnerText;
						break;
					case NODE_WEAPON_DATA_DAMAGE:
						if(node.InnerText == CONST_WEAPON_DATA_DAMAGE_KILL)
							damage = Lives.DAMAGE_KILL;
						else
							int.TryParse(node.InnerText, out damage);
						break;
					case NODE_WEAPON_DATA_RADIUS:
						float.TryParse(node.InnerText, out radius);
						break;
					case NODE_WEAPON_DATA_COOLDOWN:
						float.TryParse(node.InnerText, out cooldown);
						break;
				}
			}

			weaponData = new WeaponData(id, damage, radius, cooldown);
		}
		else if(idWeaponDataAllowed)
		{
			string weaponID = root.InnerText;
			WeaponDatabaseParser.ParseXml(DatabaseContents.GetWeaponsDatabaseText()).TryGetData(weaponID, out weaponData);

			if(string.IsNullOrEmpty(weaponData.DataID))
			{
				throw new Exception(string.Format("Weapon of ID {0} has not been found in WeaponDatabase!", weaponID));
			}
		}
		else
		{
			throw new Exception("Weapon Data has only ID defined!");
		}

		Validate(weaponData);
		return weaponData;
	}

	public static void Validate(WeaponData weaponData)
	{
		if(string.IsNullOrEmpty(weaponData.DataID))
			throw new Exception("Weapon Data has no ID defined");
	}
}