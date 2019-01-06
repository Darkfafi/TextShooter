using System;
using System.Collections.Generic;
using System.Xml;

public static class XmlToTimeline
{
	public static Timeline<T> ParseXml<T>(T game, string xmlString, Func<string, BaseTimelineEventDataParser> getDataParserForTypeMethod) where T : class, IGame
	{
		XmlDocument xmlDoc = new XmlDocument();
		xmlDoc.LoadXml(xmlString);
		return ParseXml<T>(game, xmlDoc.DocumentElement, getDataParserForTypeMethod);
	}

	public static Timeline<T> ParseXml<T>(T game, XmlNode root, Func<string, BaseTimelineEventDataParser> getDataParserForTypeMethod) where T : class, IGame
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
							if(conditions.Count > 0)
							{
								try
								{
									timelineEventSlot.AddConditionalEvent(ParseNodeToPotentialEvent(innerEventNode, getDataParserForTypeMethod), conditions.ToArray());
								}
								catch(Exception e)
								{
									throw new Exception(e.Message + ". Event number " + eventNumber);
								}
							}
							else
							{
								throw new Exception("Condition Event given without conditionKey nodes. Event number " + eventNumber);
							}
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

	private static PotentialEvent ParseNodeToPotentialEvent(XmlNode eventCaseNode, Func<string, BaseTimelineEventDataParser> getDataParserForTypeMethod)
	{
		BaseTimelineEventDataParser dataParser = null;
		XmlNode xmlData = null;
		string typeString = "'<No type node found>'";
		foreach(XmlNode node in eventCaseNode.ChildNodes)
		{
			if(node.Name == "type")
			{
				typeString = node.InnerText;
				dataParser = getDataParserForTypeMethod(typeString);
			}

			if(node.Name == "data")
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

public abstract class BaseTimelineEventDataParser
{
	public BaseTimelineEventData ParseFromXmlDataNode(XmlNode xmlDataNode, out Type timelineEventType)
	{
		BaseTimelineEventData data = ParseFromXmlSpecificDataNode(xmlDataNode, out timelineEventType);

		foreach(XmlNode node in xmlDataNode)
		{
			if(node.Name == "setKey")
			{
				XmlNode setKeyTypeNode = node.Attributes["type"];
				string keyName = node.InnerText;
				bool value = true;

				XmlNode valueNode = node.Attributes["value"];
				if(valueNode != null)
				{

					if(valueNode.InnerText == "random")
					{
						value = UnityEngine.Random.Range(0, 2) == 1 ? false : true;
					}
					else
					{
						if(!bool.TryParse(valueNode.InnerText, out value))
						{
							value = true;
						}
					}
				}

				string setKeyType;
				XmlNode progressorNode = node.Attributes["progressor"];
				if(setKeyTypeNode == null)
				{
					setKeyType = "progressor";
				}
				else
				{
					setKeyType = setKeyTypeNode.InnerText;
				}

				switch(setKeyType)
				{
					case "start":
						data.AddKeyToSetAtStartEvent(keyName, value);
						break;
					case "end":
						data.AddKeyToSetAtEndEvent(keyName, value);
						break;
					case "progressor":
						data.AddKeyToSetAtEndOfProgressors(progressorNode == null ? "" : progressorNode.InnerText, keyName, value);
						break;
					default:
						data.AddKeyToSetAtStartEvent(keyName, value);
						break;
				}
			}
		}

		return data;
	}

	protected abstract BaseTimelineEventData ParseFromXmlSpecificDataNode(XmlNode xmlDataNode, out Type timelineEventType);
}
