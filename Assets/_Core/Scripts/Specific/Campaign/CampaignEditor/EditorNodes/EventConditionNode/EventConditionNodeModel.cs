using System;
using System.Collections.Generic;
using RDP.SaveLoadSystem;

namespace GameEditor
{
	public class EventConditionNodeModel : BaseModel, ISaveable
	{
		public const string STORAGE_CONDITIONS_KEY = "ConditionsKey";
		public const string STORAGE_EVENT_NODE_KEY = "EventNodeKey";

		public ConditionItemModel[] ConditionItemModels
		{
			get
			{
				return _conditionItemModels.ToArray();
			}
		}

		public EventNodeModel EventNodeModel
		{
			get; private set;
		}

		private List<ConditionItemModel> _conditionItemModels = new List<ConditionItemModel>();
		private PopupManagerModel _popupManagerModel;
		private CampaignEditorKeys _campaignEditorKeys;

		public EventConditionNodeModel()
		{
			EventNodeModel = new EventNodeModel();
			LoadingCompleted();
		}

		public EventConditionNodeModel(IStorageLoader loader)
		{
			loader.LoadRefs<ConditionItemModel>(STORAGE_CONDITIONS_KEY, (refs) => _conditionItemModels.AddRange(refs));
			loader.LoadRef<EventNodeModel>(STORAGE_EVENT_NODE_KEY, (i) => EventNodeModel = i);
		}

		public void Save(IStorageSaver saver)
		{
			saver.SaveRefs(STORAGE_CONDITIONS_KEY, ConditionItemModels);
			saver.SaveRef(STORAGE_EVENT_NODE_KEY, EventNodeModel);
		}

		public ConditionItemModel GetConditionWithKey(string key)
		{
			for(int i = _conditionItemModels.Count - 1; i >= 0; i--)
			{
				if(_conditionItemModels[i].ConditionKey == key)
					return _conditionItemModels[i];
			}

			return null;
		}

		public string[] GetCurrentlyUsingKeys()
		{
			string[] returnValue = new string[_conditionItemModels.Count];
			for(int i = _conditionItemModels.Count - 1; i >= 0; i--)
			{
				returnValue[i] = _conditionItemModels[i].ConditionKey;
			}
			return returnValue;
		}

		public void Init(PopupManagerModel popupManagerModel, CampaignEditorKeys campaignEditorKeys)
		{
			if(_popupManagerModel != null)
				return;

			_popupManagerModel = popupManagerModel;
			_campaignEditorKeys = campaignEditorKeys;
		}

		public bool HasConditionWithKey(string key)
		{
			return GetConditionWithKey(key) != null;
		}

		public void RequestAddConditionPopup()
		{
			string[] currentlyDisplayingKeys = _campaignEditorKeys.GetKeysExcluding(GetCurrentlyUsingKeys());
			_popupManagerModel.RequestPopup(new SelectionPopupModel("Select Key", 
				(index) => 
				{
					if(index != SelectionPopupModel.ON_NO_SELECTION_INDEX)
					{
						OnKeySelected(currentlyDisplayingKeys[index]);
					}
				}
			, currentlyDisplayingKeys));
		}

		private void OnKeySelected(string key)
		{
			if(_campaignEditorKeys.HasKey(key))
			{
				AddCondition(key);
			}
		}

		private ConditionItemModel AddCondition(string key, bool value = true)
		{
			if(!HasConditionWithKey(key))
			{
				ConditionItemModel condition = new ConditionItemModel(key, value);
				_conditionItemModels.Add(condition);
				return condition;
			}

			return null;
		}

		private void RemoveCondition(string key)
		{
			ConditionItemModel item = GetConditionWithKey(key);
			if(item != null)
			{
				_conditionItemModels.Remove(item);
				item.Destroy();
			}
		}

		public void LoadingCompleted()
		{

		}

		protected override void OnModelDestroy()
		{
			EventNodeModel.Destroy();
			EventNodeModel = null;

			for(int i = 0; i < _conditionItemModels.Count; i++)
			{
				_conditionItemModels[i].Destroy();
			}

			_conditionItemModels.Clear();

			_popupManagerModel = null;
			_campaignEditorKeys = null;
		}
	}
}