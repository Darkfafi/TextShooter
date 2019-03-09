using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameEditor
{
	public class SelectionPopupView : BasePopupView
	{
		[SerializeField]
		private Text _popupTitleText;

		[SerializeField]
		private Transform _searchRowsHolder;

		[SerializeField]
		private SearchRow _searchRowPrefab;

		private SelectionPopupModel _selectionPopupModel;

		private SearchRow[] _generatedRows;

		protected override void OnViewReady()
		{
			_selectionPopupModel = MVCUtil.GetModel<SelectionPopupModel>(this);
			_popupTitleText.text = _selectionPopupModel.Title;
			_generatedRows = new SearchRow[_selectionPopupModel.SelectionItems.Length];
			for(int i = 0, c = _generatedRows.Length; i < c; i++)
			{
				SearchRow row = Instantiate(_searchRowPrefab, _searchRowsHolder);
				row.Init(_selectionPopupModel.SelectionItems[i], OnRowSelected);
				_generatedRows[i] = row;
			}
		}

		protected override void OnViewDestroy()
		{
			_generatedRows = null;
			_selectionPopupModel = null;
		}

		private void OnRowSelected(SearchRow rowSelected)
		{
			_selectionPopupModel.SelectItem(Array.IndexOf(_generatedRows, rowSelected));
		}
	}
}