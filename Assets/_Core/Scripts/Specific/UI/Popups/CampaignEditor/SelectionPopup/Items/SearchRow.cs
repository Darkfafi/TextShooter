using System;
using UnityEngine;
using UnityEngine.UI;

namespace GameEditor
{
	public class SearchRow : MonoBehaviour
	{
		[SerializeField]
		private Text _title;

		[SerializeField]
		private Button _selectButton;

		public void Init(string title, Action<SearchRow> selectButtonCallback)
		{
			_title.text = title;
			_selectButton.onClick.AddListener(() =>
			{
				selectButtonCallback(this);
			});
		}

		protected void OnDestroy()
		{
			_selectButton.onClick.RemoveAllListeners();
		}
	}
}