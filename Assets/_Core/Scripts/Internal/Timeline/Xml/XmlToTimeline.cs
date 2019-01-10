using System;
using System.Collections.Generic;
using System.Xml;

public static class XmlToTimeline
{
	public static Timeline<T> ParseXml<T>(T game, string xmlString, Func<string, BaseTimelineEventDataParser> getDataParserForTypeMethod) where T : class, IGame
	{
		XmlDocument xmlDoc = new XmlDocument();
		xmlDoc.LoadXml(xmlString);
		return ParseXml(game, xmlDoc.DocumentElement, getDataParserForTypeMethod);
	}

	public static Timeline<T> ParseXml<T>(T game, XmlNode root, Func<string, BaseTimelineEventDataParser> getDataParserForTypeMethod) where T : class, IGame
	{
		Timeline<T> timeline = new Timeline<T>(game);
		int eventNumber = 0;
		foreach(XmlNode node in root.ChildNodes)
		{
			if(node.Name == TimelineInternalGlobals.NODE_EVENT)
			{
				TimelineEventSlot<T> timelineEventSlot = null;
				try
				{
					timelineEventSlot = ParseEventNodeToTimelineSlot<T>(node, getDataParserForTypeMethod);
					if(timelineEventSlot != null)
					{
						timeline.EnqueueTimelineSlot(timelineEventSlot);
						eventNumber++;
					}
					else
					{
						throw new Exception("TimelineEventSlot was null. Event number " + eventNumber);
					}
				}
				catch(Exception e)
				{
					throw new Exception(e.Message + ". Event number " + eventNumber);
				}
			}
		}

		return timeline;
	}

	public static TimelineEventSlot<T>ParseEventNodeToTimelineSlot<T>(XmlNode eventNode, Func<string, BaseTimelineEventDataParser> getDataParserForTypeMethod) where T : class, IGame
	{
		TimelineEventSlot<T> timelineEventSlot = null;

		foreach(XmlNode innerEventNode in eventNode)
		{
			if(innerEventNode.Name == TimelineInternalGlobals.NODE_EVENT_DEFAULT)
			{
				if(timelineEventSlot == null)
				{
					timelineEventSlot = new TimelineEventSlot<T>(ParseNodeToPotentialEvent(innerEventNode, getDataParserForTypeMethod));
				}
				else
				{
					throw new Exception("Event may only have 1 default node.");
				}
			}
			else if(innerEventNode.Name == TimelineInternalGlobals.NODE_EVENT_CONDITION)
			{
				if(timelineEventSlot != null)
				{
					List<KeyValuePair<string, bool>> conditions = new List<KeyValuePair<string, bool>>();
					foreach(XmlNode innerConditionEvent in innerEventNode)
					{
						if(innerConditionEvent.Name == TimelineInternalGlobals.NODE_EVENT_CONDITION_KEY)
						{
							bool condition;
							XmlNode valueNode = innerConditionEvent.Attributes[TimelineInternalGlobals.ATTRIBUTE_EVENT_CONDITION_KEY_VALUE];
							string valueString = valueNode == null ? "true" : valueNode.InnerText;
							if(!bool.TryParse(valueString, out condition))
							{
								condition = true;
							}

							conditions.Add(new KeyValuePair<string, bool>(innerConditionEvent.InnerText, condition));
						}
					}
					if(conditions.Count > 0)
					{
						timelineEventSlot.AddConditionalEvent(ParseNodeToPotentialEvent(innerEventNode, getDataParserForTypeMethod), conditions.ToArray());
					}
					else
					{
						throw new Exception("Condition Event given without conditionKey nodes.");
					}
				}
				else
				{
					throw new Exception("Condition Event came before Default event.");
				}
			}
		}

		if(timelineEventSlot != null)
		{
			return timelineEventSlot;
		}
		else
		{
			throw new Exception("Timeline could not add event due to it not having a default nor condition tags.");
		}
	}

	public static PotentialEvent ParseNodeToPotentialEvent(XmlNode eventCaseNode, Func<string, BaseTimelineEventDataParser> getDataParserForTypeMethod)
	{
		BaseTimelineEventDataParser dataParser = null;
		XmlNode xmlData = null;
		string typeString = TimelineInternalGlobals.CONST_EVENT_INTERNAL_DATA_TYPE_NONE_FOUND;
		foreach(XmlNode node in eventCaseNode.ChildNodes)
		{
			if(node.Name == TimelineInternalGlobals.NODE_EVENT_INTERNAL_TYPE)
			{
				typeString = node.InnerText;
				dataParser = getDataParserForTypeMethod(typeString);
			}

			if(node.Name == TimelineInternalGlobals.NODE_EVENT_INTERNAL_DATA)
			{
				xmlData = node;
			}
		}

		if(xmlData == null)
		{
			throw new Exception("No data node given to the event");
		}

		if(dataParser == null)
		{
			throw new Exception("No data parser found for the event type " + typeString);
		}

		Type timelineEventType;
		BaseTimelineEventData data = dataParser.ParseFromXmlDataNode(xmlData, out timelineEventType);
		return PotentialEvent.Create(timelineEventType, data);
	}
}