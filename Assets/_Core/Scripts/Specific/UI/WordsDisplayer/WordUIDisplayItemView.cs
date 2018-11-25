using UnityEngine;
using UnityEngine.UI;

public class WordUIDisplayItemView : EntityView
{
    [SerializeField]
    private Text _wordTextDisplay;

	private WordUIDisplayItemModel _wordUIDisplayItemModel;

	private CameraView _gameCamera;

	protected override void OnViewReady()
	{
		_wordUIDisplayItemModel = MVCUtil.GetModel<WordUIDisplayItemModel>(this);
		_gameCamera = MVCUtil.GetView<CameraView>(EntityTracker.Instance.GetFirst<CameraModel>());
		IgnoreModelTransform = true;
	}

	protected override void Update()
	{
		base.Update();

		if(_wordUIDisplayItemModel != null)
		{
			Vector2 pos = _gameCamera.Camera.WorldToScreenPoint(_wordUIDisplayItemModel.ModelTransform.Position);
			transform.position = pos;
		}
	}
}
