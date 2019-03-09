using System;
using UnityEngine;

namespace GameEditor
{
	public class SelectionPopupModel : BasePopupModel
	{
		[PopupID]
		public const string POPUP_ID = "SelectionPopup";

		public const int ON_NO_SELECTION_INDEX = -1;

		public override string PopupModelID
		{
			get
			{
				return POPUP_ID;
			}
		}

		public string Title
		{
			get; private set;
		}

		public string[] SelectionItems
		{
			get; private set;
		}

		private Action<int> _selectionCallback;

		public SelectionPopupModel(string title, Action<int> selectionCallback, params string[] selectionItems)
		{
			Title = title;
			SelectionItems = selectionItems;
			_selectionCallback = selectionCallback;
		}

		public void SelectItem(int index)
		{
			index = Mathf.Clamp(index, 0, SelectionItems.Length - 1);
			_selectionCallback(index);
			Close();
		}

		protected override void OnClose()
		{
			_selectionCallback(ON_NO_SELECTION_INDEX);
			base.OnClose();
		}
	}
}