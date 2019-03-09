using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameEditor
{
	public class ConditionItemView : MonoBaseView
	{
		[SerializeField]
		private InputField _inputField;

		[SerializeField]
		private Dropdown _choiceDropdown;

		private ConditionItemModel _conditionItemModel;

		private List<string> _options = new List<string>()
		{
			"True",
			"False"
		};

		protected override void OnViewReady()
		{
			_conditionItemModel = MVCUtil.GetModel<ConditionItemModel>(this);
			_choiceDropdown.onValueChanged.AddListener(OnValueChangedEvent);
			_choiceDropdown.ClearOptions();
			_choiceDropdown.AddOptions(_options);

			_inputField.text = _conditionItemModel.ConditionKey;
			_choiceDropdown.value = BoolToIndex(_conditionItemModel.ConditionValue);
		}

		protected override void OnViewDestroy()
		{
			_choiceDropdown.onValueChanged.RemoveAllListeners();
			_conditionItemModel = null;
		}

		private void OnValueChangedEvent(int index)
		{
			_conditionItemModel.ConditionValue = IndexToBool(index);
		}

		private bool IndexToBool(int index)
		{
			if(index < 0 || index > _options.Count - 1)
				return false;

			return bool.Parse(_options[index]);
		}

		private int BoolToIndex(bool value)
		{
			return value ? 0 : 1;
		}
	}
}