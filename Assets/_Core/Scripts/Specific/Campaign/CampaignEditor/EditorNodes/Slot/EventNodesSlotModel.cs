using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RDP.SaveLoadSystem;
using System;

namespace GameEditor
{
	public class EventNodesSlotModel : BaseModel, ISaveable
	{
		public const string STORAGE_DEFAULT_NODE_KEY = "DefaultNodeKey";
		public const string STORAGE_CONDITIONAL_NODES_KEY = "ConditionalNodesKey";

		public EventNodeModel DefaultNodeModel
		{
			get; private set;
		}

		public List<ConditionalEventNodeHolder> ConditionalEventNodes
		{
			get; private set;
		}

		public EventNodesSlotModel()
		{
			ConditionalEventNodes = new List<ConditionalEventNodeHolder>();
			LoadingCompleted();
		}

		public EventNodesSlotModel(IStorageLoader loader)
		{
			ConditionalEventNodes = new List<ConditionalEventNodeHolder>();
			loader.LoadRef<EventNodeModel>(STORAGE_DEFAULT_NODE_KEY, (i) => DefaultNodeModel = i);
			loader.LoadRefs<ConditionalEventNodeHolder>(STORAGE_CONDITIONAL_NODES_KEY, (i) => ConditionalEventNodes.AddRange(i));
		}

		public void Save(IStorageSaver saver)
		{
			saver.SaveRef(STORAGE_DEFAULT_NODE_KEY, DefaultNodeModel);
			saver.SaveRefs(STORAGE_CONDITIONAL_NODES_KEY, ConditionalEventNodes.ToArray());
		}

		public void LoadingCompleted()
		{
			if(ConditionalEventNodes.Count == 0)
				ConditionalEventNodes.Add(new ConditionalEventNodeHolder("lol"));
		}
	}

	public class ConditionalEventNodeHolder : ISaveableLoad
	{
		public const string STORAGE_CONDITIONS_KEY = "ConditionsKey";
		public const string STORAGE_NODE_KEY = "NodeKey";

		public Dictionary<string, bool> KeyConditions = new Dictionary<string, bool>();
		public EventNodeModel ConditionNodeModel;

		public ConditionalEventNodeHolder()
		{

		}

		public ConditionalEventNodeHolder(string key)
		{
			KeyConditions.Add(key, true);
		}

		public void Save(IStorageSaver saver)
		{
			saver.SaveRef(STORAGE_NODE_KEY, ConditionNodeModel);
			saver.SaveDict(STORAGE_CONDITIONS_KEY, KeyConditions);
		}

		public void Load(IStorageLoader loader)
		{
			loader.LoadRef<EventNodeModel>(STORAGE_NODE_KEY, (i) => ConditionNodeModel = i);
			loader.LoadDict(STORAGE_CONDITIONS_KEY, out KeyConditions);
		}

		public void LoadingCompleted()
		{
			if(KeyConditions == null)
				KeyConditions = new Dictionary<string, bool>();
		}
	}
}