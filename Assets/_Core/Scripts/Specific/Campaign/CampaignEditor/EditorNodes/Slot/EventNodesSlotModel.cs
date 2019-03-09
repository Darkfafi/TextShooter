using System.Collections.Generic;
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

		public EventConditionNodeModel[] ConditionalEventNodes
		{
			get
			{
				return _conditionalEventNodes.ToArray();
			}
		}

		private List<EventConditionNodeModel> _conditionalEventNodes;

		public EventNodesSlotModel()
		{
			_conditionalEventNodes = new List<EventConditionNodeModel>();
			DefaultNodeModel = new EventNodeModel();
			LoadingCompleted();
		}

		protected override void OnModelDestroy()
		{
			for(int i = 0; i < _conditionalEventNodes.Count; i++)
			{
				_conditionalEventNodes[i].Destroy();
			}

			_conditionalEventNodes.Clear();
			_conditionalEventNodes = null;

			DefaultNodeModel.Destroy();
			DefaultNodeModel = null;
		}

		public EventNodesSlotModel(IStorageLoader loader)
		{
			_conditionalEventNodes = new List<EventConditionNodeModel>();
			loader.LoadRef<EventNodeModel>(STORAGE_DEFAULT_NODE_KEY, (i) => DefaultNodeModel = i);
			loader.LoadRefs<EventConditionNodeModel>(STORAGE_CONDITIONAL_NODES_KEY, (i) => _conditionalEventNodes = new List<EventConditionNodeModel>(i));
		}

		public void Save(IStorageSaver saver)
		{
			saver.SaveRef(STORAGE_DEFAULT_NODE_KEY, DefaultNodeModel);
			saver.SaveRefs(STORAGE_CONDITIONAL_NODES_KEY, _conditionalEventNodes.ToArray());
		}

		public void LoadingCompleted()
		{

		}

		public EventConditionNodeModel AddConditionalEventNode()
		{
			EventConditionNodeModel model = new EventConditionNodeModel();
			_conditionalEventNodes.Add(model);
			return model;
		}
	}
}