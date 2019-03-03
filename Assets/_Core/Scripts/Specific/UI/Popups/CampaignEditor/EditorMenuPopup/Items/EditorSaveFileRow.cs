using System;
using UnityEngine;
using UnityEngine.UI;

namespace GameEditor
{
	public class EditorSaveFileRow : MonoBehaviour
	{
		[SerializeField]
		private Text _nameText;

		[SerializeField]
		private Text _descriptionText;

		[SerializeField]
		private Button _loadButton;

		[SerializeField]
		private Button _removeButton;

		public void SetupRowItem(string name, string description, Action<EditorSaveFileRow> loadButtonCallback, Action<EditorSaveFileRow> removeButtonCallback)
		{
			_nameText.text = name;
			_descriptionText.text = description;
			_loadButton.onClick.AddListener(() => loadButtonCallback(this));
			_removeButton.onClick.AddListener(() => removeButtonCallback(this));
		}

		protected void OnDestroy()
		{
			_loadButton.onClick.RemoveAllListeners();
			_removeButton.onClick.RemoveAllListeners();
		}
	}
}