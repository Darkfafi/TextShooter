using UnityEngine;

public class WordsDisplayerView : MonoBaseView
{
    [SerializeField]
    private WordUIDisplayItemView _wordUIDisplayItemPrefab;

	[SerializeField]
	private Canvas _gameCanvas;

	private WordsDisplayerModel _wordsDisplayerModel;

	protected override void OnViewReady()
	{
		_wordsDisplayerModel = MVCUtil.GetModel<WordsDisplayerModel>(this);
		_wordsDisplayerModel.AddedDisplayItemEvent += OnAddedDisplayItemEvent;
	}

	protected override void OnViewDestroy()
	{
		_wordsDisplayerModel.AddedDisplayItemEvent -= OnAddedDisplayItemEvent;
		_wordsDisplayerModel = null;
	}

	private void OnAddedDisplayItemEvent(WordUIDisplayItemModel item)
	{
		WordUIDisplayItemView itemView = Instantiate(_wordUIDisplayItemPrefab);
		Controller.Link(item, itemView);
		itemView.transform.SetParent(_gameCanvas.transform);
	}
}
