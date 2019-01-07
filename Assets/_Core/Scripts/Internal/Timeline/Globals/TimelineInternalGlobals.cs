public static class TimelineInternalGlobals
{
	// -- XML -- \\
	// Event
	public const string NODE_EVENT = "event";
	// Event/Default
	public const string NODE_EVENT_DEFAULT = "default";
	// Event/Condition
	public const string NODE_EVENT_CONDITION = "condition";
	public const string NODE_EVENT_CONDITION_KEY = "conditionKey";
	public const string ATTRIBUTE_EVENT_CONDITION_KEY_VALUE = "value";
	// Event/(Default or Condition)
	public const string NODE_EVENT_INTERNAL_TYPE = "type";
	public const string NODE_EVENT_INTERNAL_DATA = "data";
	// Event/(Default or Condition)/Data
	public const string NODE_EVENT_INTERNAL_DATA_SET_KEY = "setKey";

	public const string ATTRIBUTE_EVENT_INTERNAL_DATA_SET_KEY_TYPE = "type";
	public const string CONST_EVENT_INTERNAL_DATA_SET_KEY_TYPE_NONE_FOUND = "<No type node found>";
	public const string CONST_EVENT_INTERNAL_DATA_SET_KEY_TYPE_START = "start";
	public const string CONST_EVENT_INTERNAL_DATA_SET_KEY_TYPE_END = "end";
	public const string CONST_EVENT_INTERNAL_DATA_SET_KEY_TYPE_PROGRESSOR = "progressor";

	public const string ATTRIBUTE_EVENT_INTERNAL_DATA_SET_KEY_VALUE = "value";
	public const string CONST_EVENT_INTERNAL_DATA_SET_KEY_VALUE_RANDOM = "random";

	public const string ATTRIBUTE_EVENT_INTERNAL_DATA_SET_KEY_PROGRESSOR = "progressor";

	// Event/(Default or Condition)/Data
	public const string NODE_EVENT_INTERNAL_DATA_PROGRESSOR = "progressor";
	public const string ATTRIBUTE_EVENT_INTERNAL_DATA_PROGRESSOR_VALUE = "value";
}
