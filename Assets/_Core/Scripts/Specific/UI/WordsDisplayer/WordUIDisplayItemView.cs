using DG.Tweening;
using System;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class WordUIDisplayItemView : EntityView
{
	public string DisplayingWord
	{
		get
		{
			if(_wordUIDisplayItemModel == null)
				return "";

			return _wordUIDisplayItemModel.CurrentlyDisplayingWord;
		}
	}

	[SerializeField]
	private Text _wordTextDisplay;

	private CameraView _gameCamera;
	private WordUIDisplayItemModel _wordUIDisplayItemModel;

	private bool playingDisplayAnimation = false;
	private bool displaying = false;

	private Action _lastTextColorApplyMethod;

	public void SetTextColor(int charIndexStart, int charIndexEnd, string colorCode)
	{
		_wordTextDisplay.supportRichText = true;
		charIndexEnd = charIndexEnd + 1;
		int length = charIndexEnd - charIndexStart;
		_lastTextColorApplyMethod = () =>
		{
			StringBuilder wordBuilder = new StringBuilder(DisplayingWord);
			string subString = DisplayingWord.Substring(charIndexStart, length);
			wordBuilder.Replace(subString, string.Format("<color=#{0}>{1}</color>", colorCode, subString), charIndexStart, subString.Length);
			_wordTextDisplay.text = wordBuilder.ToString();
		};

		_lastTextColorApplyMethod();
	}

	public void ResetTextColor()
	{
		_wordTextDisplay.text = DisplayingWord;
		_lastTextColorApplyMethod = null;
	}

	protected override void OnViewReady()
	{
		base.OnViewReady();
		_wordUIDisplayItemModel = MVCUtil.GetModel<WordUIDisplayItemModel>(this);
		_gameCamera = MVCUtil.GetView<CameraView>(EntityTracker.Instance.GetFirst<CameraModel>());
		IgnoreModelTransform = true;

		transform.localScale = Vector2.zero;
		_wordUIDisplayItemModel.NewWordDisplayingEvent += OnNewWordDisplayingEvent;
	}

	protected override void OnViewDestroy()
	{
		base.OnViewDestroy();
		_wordUIDisplayItemModel.NewWordDisplayingEvent -= OnNewWordDisplayingEvent;
		_wordUIDisplayItemModel = null;
		_gameCamera = null;
		_wordTextDisplay = null;
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

	protected override void ViewDestruction()
	{
		if(!displaying)
		{
			base.ViewDestruction();
		}
		else
		{
			HideAnimationAnimation(() => { base.ViewDestruction(); });
		}
	}

	private void DisplayAnimation()
	{
		if(playingDisplayAnimation || displaying)
		{
			return;
		}

		playingDisplayAnimation = true;
		transform.DOComplete(true);
		_wordTextDisplay.text = "";
		_wordTextDisplay.rectTransform.sizeDelta = Vector2.zero;
		transform.localScale = Vector3.zero;
		DoTextAnimation(0.3f, 0.2f);
		transform.DOScale(1, 0.5f).SetEase(Ease.OutBack).OnComplete(() =>
		{
			playingDisplayAnimation = false;
			displaying = true;
		});
	}

	private void HideAnimationAnimation(Action callback = null)
	{
		if(!displaying)
		{
			return;
		}

		displaying = false;
		transform.DOComplete(true);
		transform.DOScale(0, 0.4f).SetEase(Ease.InBack).OnComplete(() => { if(callback != null) { callback(); } });
	}

	private void OnNewWordDisplayingEvent(string newWord)
	{
		_lastTextColorApplyMethod = null;
		DoTextAnimation(0.3f, 0);
		if(string.IsNullOrEmpty(newWord))
		{
			_wordTextDisplay.rectTransform.DOSizeDelta(Vector2.zero, 0.2f);
		}
	}

	private void DoTextAnimation(float duration, float delay)
	{
		_wordTextDisplay.DOText(_wordUIDisplayItemModel.CurrentlyDisplayingWord, duration).SetDelay(delay).OnComplete(()=> 
		{
			DoFinalColor();
		});
	}

	private void DoFinalColor()
	{
		if(_lastTextColorApplyMethod != null)
		{
			_lastTextColorApplyMethod();
			_lastTextColorApplyMethod = null;
		}
	}
}
