using UnityEngine;
using UnityEngine.UI;

namespace GameEditor
{
	public class EventNodesSlotView : MonoBaseView
	{
		[SerializeField]
		private Transform _nodeViewsHolder;

		[SerializeField]
		private Button _addConditionalEventButton;

		[SerializeField]
		private EventNodeView _eventNodeViewPrefab;

		[SerializeField]
		private EventConditionNodeView _eventConditionNodeViewPrefab;

		private EventNodesSlotModel _eventNodesSlotModel;

		protected override void OnViewReady()
		{
			_eventNodesSlotModel = MVCUtil.GetModel<EventNodesSlotModel>(this);
			CreateViewForDefaultNode();

			for(int i = 0; i < _eventNodesSlotModel.ConditionalEventNodes.Length; i++)
			{
				CreateViewForConditionalNode(_eventNodesSlotModel.ConditionalEventNodes[i]);
			}

			_addConditionalEventButton.onClick.AddListener(OnAddConditionalEventButtonPressed);
		}

		protected override void OnViewDestroy()
		{
			_eventNodesSlotModel = null;
		}

		private void CreateViewForDefaultNode()
		{
			if(!MVCUtil.HasView(_eventNodesSlotModel.DefaultNodeModel))
			{
				EventNodeView view = Instantiate(_eventNodeViewPrefab, _nodeViewsHolder);
				Controller.Link(_eventNodesSlotModel.DefaultNodeModel, view);
			}
		}

		private void CreateViewForConditionalNode(EventConditionNodeModel model)
		{
			if(!MVCUtil.HasView(model))
			{
				EventConditionNodeView view = Instantiate(_eventConditionNodeViewPrefab, _nodeViewsHolder);
				Controller.Link(model, view);
			}
		}

		private void OnAddConditionalEventButtonPressed()
		{
			CreateViewForConditionalNode(_eventNodesSlotModel.AddConditionalEventNode());
		}
	}
}