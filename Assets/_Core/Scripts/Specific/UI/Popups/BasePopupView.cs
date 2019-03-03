using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public abstract class BasePopupView : MonoBaseView
{
	protected Button OptionalCloseButton
	{
		get
		{
			return _optionalCloseButton;
		}
	}

	[SerializeField]
	private Button _optionalCloseButton;

	public override void DestroyView()
	{
		transform.DOScale(0, 0.18f).OnComplete(() =>
		{
			base.DestroyView();
		});
	}

	protected void Awake()
	{
		Vector3 preScale = transform.localScale;
		transform.localScale = Vector2.zero;
		transform.DOScale(preScale, 0.18f);

		if(_optionalCloseButton != null)
		{
			_optionalCloseButton.onClick.AddListener(OnCloseClicked);
		}
	}

	protected void OnDestroy()
	{
		if(_optionalCloseButton != null)
		{
			_optionalCloseButton.onClick.RemoveAllListeners();
		}
	}

	private void OnCloseClicked()
	{
		if(MVCUtil.HasModel(this))
		{
			MVCUtil.GetModel<BasePopupModel>(this).Close();
		}
	}
}
