using RDP.SaveLoadSystem;

namespace GameEditor
{
	public class ConditionItemModel : BaseModel, ISaveable
	{
		public const string STORAGE_CONDITION_KEY_KEY = "ConditionKeyKey";
		public const string STORAGE_CONDITION_VALUE_KEY = "ConditionValueKey";

		public string ConditionKey
		{
			get; private set;
		}

		public bool ConditionValue;

		public ConditionItemModel(IStorageLoader loader)
		{
			ConditionKey = loader.LoadValue<string>(STORAGE_CONDITION_KEY_KEY);
			loader.LoadValue(STORAGE_CONDITION_VALUE_KEY, out ConditionValue);
		}

		public ConditionItemModel(string key, bool value)
		{
			ConditionKey = key;
			ConditionValue = value;
		}

		public void Save(IStorageSaver saver)
		{
			saver.SaveValue(STORAGE_CONDITION_KEY_KEY, ConditionKey);
			saver.SaveValue(STORAGE_CONDITION_VALUE_KEY, ConditionValue);
		}

		public void LoadingCompleted()
		{

		}
	}
}