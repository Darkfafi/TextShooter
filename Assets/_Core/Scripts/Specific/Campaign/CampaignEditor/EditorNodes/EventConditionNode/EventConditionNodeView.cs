using UnityEngine;
using UnityEngine.UI;

namespace GameEditor
{
	public class EventConditionNodeView : MonoBaseView
	{
		[SerializeField]
		private Transform _conditionsHolder;

		[SerializeField]
		private Transform _eventNodeViewHolder;

		[SerializeField]
		private Button _addConditionButton;

		[SerializeField]
		private ConditionItemView _conditionItemViewPrefab; 

		[SerializeField]
		private EventNodeView _eventNodeViewPrefab;

		private EventConditionNodeModel _eventConditionModel;

		protected override void OnViewReady()
		{
			_eventConditionModel = MVCUtil.GetModel<EventConditionNodeModel>(this);
			CreateNodeView();

			for(int i = 0; i < _eventConditionModel.ConditionItemModels.Length; i++)
			{
				CreateConditionView(_eventConditionModel.ConditionItemModels[i]);
			}

			_addConditionButton.onClick.AddListener(OnAddConditionButtonPressed);
		}

		protected override void OnViewDestroy()
		{
			_addConditionButton.onClick.RemoveListener(OnAddConditionButtonPressed);
			_eventConditionModel = null;
		}

		private void OnAddConditionButtonPressed()
		{
#warning TODO: Open Condition Item Creation Popup (with make key / select key (open search popup) functionality)
			//_eventConditionModel. Open Condition Creation popup and 
		}

		private void CreateConditionView(ConditionItemModel model)
		{
			if(!MVCUtil.HasView(model))
			{
				ConditionItemView view = Instantiate(_conditionItemViewPrefab, _conditionsHolder);
				Controller.Link(model, view);
			}
		}

		private void CreateNodeView()
		{
			if(!MVCUtil.HasView(_eventConditionModel.EventNodeModel))
			{
				EventNodeView view = Instantiate(_eventNodeViewPrefab, _eventNodeViewHolder);
				Controller.Link(_eventConditionModel.EventNodeModel, view);
			}
		}
	}
}