using System;
using System.Collections.Generic;
using System.Xml;

public static class XmlToTimeline
{
	public static Timeline<T> ParseXml<T>(T game, string xmlString, Func<string, ITimelineEventDataParser> getDataParserForTypeMethod) where T : class, IGame
	{
		XmlDocument xmlDoc = new XmlDocument();
		xmlDoc.LoadXml(xmlString);
		return ParseXml<T>(game, xmlDoc.DocumentElement, getDataParserForTypeMethod);
	}

	public static Timeline<T> ParseXml<T>(T game, XmlNode root, Func<string, ITimelineEventDataParser> getDataParserForTypeMethod) where T : class, IGame
	{
		Timeline<T> timeline = new Timeline<T>(game);
		int eventNumber = 0;
		foreach(XmlNode node in root.ChildNodes)
		{
			if(node.Name == "event")
			{
				TimelineEventSlot<T> timelineEventSlot = null;

				foreach(XmlNode innerEventNode in node)
				{
					if(innerEventNode.Name == "default")
					{
						timelineEventSlot = new TimelineEventSlot<T>(ParseNodeToPotentialEvent(innerEventNode, getDataParserForTypeMethod));
					}
					else if(innerEventNode.Name == "condition")
					{
						if(timelineEventSlot != null)
						{
							List<KeyValuePair<string, bool>> conditions = new List<KeyValuePair<string, bool>>();
							foreach(XmlNode innerConditionEvent in innerEventNode)
							{
								if(innerConditionEvent.Name == "conditionKey")
								{
									bool condition;
									if(!bool.TryParse(innerConditionEvent.Value, out condition))
									{
										condition = true;
									}

									conditions.Add(new KeyValuePair<string, bool>(innerConditionEvent.InnerText, condition));
								}
							}
							timelineEventSlot.AddConditionalEvent(ParseNodeToPotentialEvent(innerEventNode, getDataParserForTypeMethod), conditions.ToArray());
						}
						else
						{
							throw new Exception("Condition Event came before Default event. Event number " + eventNumber);
						}
					}
				}

				if(timelineEventSlot != null)
				{
					timeline.EnqueueTimelineSlot(timelineEventSlot);
					eventNumber++;
				}
				else
				{
					throw new Exception("Timeline could not add event due to it not having a default nor condition tags. Event number " + eventNumber);
				}
			}
		}

		return timeline;
	}

	private static PotentialEvent ParseNodeToPotentialEvent(XmlNode eventCaseNode, Func<string, ITimelineEventDataParser> getDataParserForTypeMethod)
	{
		ITimelineEventDataParser dataParser = null;
		XmlNode xmlData = null;
		foreach(XmlNode node in eventCaseNode.ChildNodes)
		{
			if(node.Name == "type")
			{
				dataParser = getDataParserForTypeMethod(node.InnerText);
			}

			if(node.Name == "data")
			{
				xmlData = node;
			}
		}

		if(dataParser != null && xmlData != null)
		{
			Type timelineEventType;
			ITimelineEventData data = dataParser.ParseFromXmlDataNode(xmlData, out timelineEventType);
			return PotentialEvent.Create(timelineEventType, data);
		}

		return null;
	}
}

public interface ITimelineEventDataParser
{
	ITimelineEventData ParseFromXmlDataNode(XmlNode xmlDataNode, out Type timelineEventType);
}
