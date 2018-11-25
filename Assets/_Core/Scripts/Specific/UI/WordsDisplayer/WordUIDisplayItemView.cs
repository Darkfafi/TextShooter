using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;

public class WordUIDisplayItemView : EntityView
{
    [SerializeField]
    private Text _wordTextDisplay;

	private CameraView _gameCamera;

	private bool playingDisplayAnimation = false;
	private bool displaying = false;

	protected override void OnViewReady()
	{
		base.OnViewReady();
		_gameCamera = MVCUtil.GetView<CameraView>(EntityTracker.Instance.GetFirst<CameraModel>());
		IgnoreModelTransform = true;

		transform.localScale = Vector2.zero;
	}

	protected override void Update()
	{
		if(SelfModel == null)
		{
			return;
		}

		base.Update();

		if(!SelfModel.IsDestroyed)
		{
			Vector2 screenPosition = _gameCamera.Camera.WorldToScreenPoint(SelfModel.ModelTransform.Position);
			Vector2 viewportPoint = _gameCamera.Camera.ScreenToViewportPoint(screenPosition);

			if(viewportPoint.x < 0 || viewportPoint.x > 1 || viewportPoint.y < 0 || viewportPoint.y > 1)
			{
				HideAnimationAnimation();
			}
			else
			{
				DisplayAnimation();
			}

			transform.position = screenPosition;
		}
	}

	protected override void OnViewDestroy()
	{
		base.OnViewDestroy();
		_gameCamera = null;
		_wordTextDisplay = null;
	}

	protected override void ViewDestruction()
	{
		if (!displaying)
		{
			base.ViewDestruction();
		}
		else
		{
			HideAnimationAnimation(()=> { base.ViewDestruction(); });
		}
	}

	private void DisplayAnimation()
	{
		if (playingDisplayAnimation || displaying)
		{
			return;
		}

		transform.DOComplete(true);
		playingDisplayAnimation = true;
		transform.localScale = Vector3.zero;
		transform.DOScale(1, 0.5f).SetEase(Ease.OutBack).OnComplete(() => 
		{
			playingDisplayAnimation = false;
			displaying = true;
		});
	}

	private void HideAnimationAnimation(Action callback = null)
	{
		if (!displaying)
		{
			return;
		}

		displaying = false;
		transform.DOComplete(true);
		transform.DOScale(0, 0.4f).SetEase(Ease.InBack).OnComplete(()=> { if (callback != null) { callback(); } });
	}
}
