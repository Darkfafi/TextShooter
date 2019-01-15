using System.Xml;

public static class XmlToCampaign
{
	public static Campaign<T> ParseXml<T>(T game, string campaignXmlString) where T : class, IGame
	{
		string name = "", description = "";
		Timeline<T> timeline = null;
		XmlDocument xmlDoc = new XmlDocument();
		xmlDoc.LoadXml(campaignXmlString);

		foreach(XmlNode node in xmlDoc.DocumentElement.ChildNodes)
		{
			if(node.Name == CampaignGlobals.NODE_NAME)
			{
				name = node.InnerText;
			}

			if(node.Name == CampaignGlobals.NODE_DESCRIPTION)
			{
				description = node.InnerText;
			}

			if(node.Name == CampaignGlobals.NODE_TIMELINE)
			{
				timeline = XmlToTimeline.ParseXml<T>(game, node, EventDataParsers.GetDataParserForType);
			}
		}

		return new Campaign<T>(new CampaignData()
		{
			Name = name,
			Description = description
		}, timeline);
	}
}
