﻿using System;
using System.Collections.Generic;
using System.Xml;

public abstract class BaseTimelineEventDataParser
{
	public BaseTimelineEventData ParseFromXmlDataNode(XmlNode xmlDataNode, out Type timelineEventType)
	{
		BaseTimelineEventData data = ParseFromXmlSpecificDataNode(xmlDataNode, out timelineEventType);

		foreach(XmlNode node in xmlDataNode)
		{
			if(node.Name == TimelineInternalGlobals.NODE_EVENT_INTERNAL_DATA_SET_KEY)
			{
				XmlNode setKeyTypeNode = node.Attributes[TimelineInternalGlobals.ATTRIBUTE_EVENT_INTERNAL_DATA_SET_KEY_TYPE];
				string keyName = node.InnerText;
				bool value = true;

				XmlNode valueNode = node.Attributes[TimelineInternalGlobals.ATTRIBUTE_EVENT_INTERNAL_DATA_SET_KEY_VALUE];
				if(valueNode != null)
				{

					if(valueNode.InnerText == TimelineInternalGlobals.CONST_EVENT_INTERNAL_DATA_SET_KEY_VALUE_RANDOM)
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

				string setKeyType = "";
				if(setKeyTypeNode != null)
				{
					setKeyType = setKeyTypeNode.InnerText;
				}
				switch(setKeyType)
				{
					case TimelineInternalGlobals.CONST_EVENT_INTERNAL_DATA_SET_KEY_TYPE_START:
						data.AddKeyToSetAtStartEvent(keyName, value);
						break;
					case TimelineInternalGlobals.CONST_EVENT_INTERNAL_DATA_SET_KEY_TYPE_END:
						data.AddKeyToSetAtEndEvent(keyName, value);
						break;
					default:
						data.AddKeyToSetAtStartEvent(keyName, value);
						break;
				}
			}
			else if(node.Name == TimelineInternalGlobals.NODE_EVENT_INTERNAL_DATA_PROGRESSOR)
			{
				BaseTimelineEventData.EventProgressorData.EventEndType endEventType = BaseTimelineEventData.EventProgressorData.EventEndType.AtGoalReach;
				if(node.Attributes[TimelineInternalGlobals.ATTRIBUTE_EVENT_INTERNAL_DATA_PROGRESSOR_EVENT_END_TYPE] != null)
				{
					switch(node.Attributes[TimelineInternalGlobals.ATTRIBUTE_EVENT_INTERNAL_DATA_PROGRESSOR_EVENT_END_TYPE].InnerText)
					{
						case TimelineInternalGlobals.CONST_EVENT_INTERNAL_DATA_PROGRESSOR_EVENT_END_TYPE_NONE:
							endEventType = BaseTimelineEventData.EventProgressorData.EventEndType.None;
							break;
						case TimelineInternalGlobals.CONST_EVENT_INTERNAL_DATA_PROGRESSOR_EVENT_END_TYPE_AT_VALUE:
							endEventType = BaseTimelineEventData.EventProgressorData.EventEndType.AtHitValue;
							break;
						default:
							endEventType = BaseTimelineEventData.EventProgressorData.EventEndType.AtGoalReach;
							break;
					}
				}

				KeyValuePair<string, bool> keyToSet = default(KeyValuePair<string, bool>);
				if(node.Attributes[TimelineInternalGlobals.ATTRIBUTE_EVENT_INTERNAL_DATA_PROGRESSOR_SET_KEY] != null)
				{
					bool keyValue = true;
					if(node.Attributes[TimelineInternalGlobals.ATTRIBUTE_EVENT_INTERNAL_DATA_PROGRESSOR_KEY_VALUE] != null)
					{
						bool.TryParse(node.Attributes[TimelineInternalGlobals.ATTRIBUTE_EVENT_INTERNAL_DATA_PROGRESSOR_KEY_VALUE].InnerText, out keyValue);
					}
					keyToSet = new KeyValuePair<string, bool>(node.Attributes[TimelineInternalGlobals.ATTRIBUTE_EVENT_INTERNAL_DATA_PROGRESSOR_SET_KEY].InnerText, keyValue);
				}

				int atValue = BaseTimelineEventData.EventProgressorData.VALUE_AT_GOAL;
				if(node.Attributes[TimelineInternalGlobals.ATTRIBUTE_EVENT_INTERNAL_DATA_PROGRESSOR_AT_VALUE] != null)
				{
					int.TryParse(node.Attributes[TimelineInternalGlobals.ATTRIBUTE_EVENT_INTERNAL_DATA_PROGRESSOR_AT_VALUE].InnerText, out atValue);
				}

				string value = node.Attributes[TimelineInternalGlobals.ATTRIBUTE_EVENT_INTERNAL_DATA_PROGRESSOR_VALUE] == null ? "" : node.Attributes[TimelineInternalGlobals.ATTRIBUTE_EVENT_INTERNAL_DATA_PROGRESSOR_VALUE].InnerText;

				BaseTimelineEventData.EventProgressorData progressorEventData = new BaseTimelineEventData.EventProgressorData()
				{
					KeyValuePairToSet = keyToSet,
					ValueToSetKeyAt = atValue,
					EndEventType = endEventType,
					OptionalStringValue = value,
				};

				data.AddProgressorByName(node.InnerText, progressorEventData);
			}
			else if(node.Name == TimelineInternalGlobals.NODE_EVENT_INTERNAL_DATA_ENDING_TYPE)
			{
				data.SetEndingType(node.InnerText);
			}
		}

		return data;
	}

	protected abstract BaseTimelineEventData ParseFromXmlSpecificDataNode(XmlNode xmlDataNode, out Type timelineEventType);
}