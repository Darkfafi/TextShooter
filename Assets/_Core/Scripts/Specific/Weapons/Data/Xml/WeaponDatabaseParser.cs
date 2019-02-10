using System;
using System.Collections.Generic;
using System.Xml;

public static class WeaponDatabaseParser
{
	public const string NODE_WEAPON_DATA = "weaponData";

	public static StaticDatabase<WeaponData> ParseXml(string xmlString)
	{
		XmlDocument xmlDoc = new XmlDocument();
		xmlDoc.LoadXml(xmlString);
		return ParseXml(xmlDoc.DocumentElement);
	}

	public static StaticDatabase<WeaponData> ParseXml(XmlNode root)
	{
		List<WeaponData> data = new List<WeaponData>();

		foreach(XmlNode node in root)
		{
			if(node.Name == NODE_WEAPON_DATA)
			{
				try
				{
					data.Add(WeaponDataParser.ParseXml(node, false));
				}
				catch(Exception e)
				{
					UnityEngine.Debug.LogError("Could not parse WeaponDatabase XML. Error: " + e.Message);
				}
			}
		}

		return new StaticDatabase<WeaponData>(data.ToArray());
	}
}