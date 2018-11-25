using UnityEngine;
using DG.Tweening;

public class TurretView : EntityView
{
	[SerializeField]
	private GameObject _turretTurningPoint;

	[SerializeField]
	private GameObject _rangeIndicator;

	private TurretModel _model;

	protected override void OnViewReady()
	{
		base.OnViewReady();
		_model = MVCUtil.GetModel<TurretModel>(this);
		_model.TargetSetEvent += OnTargetSetEvent;

		_rangeIndicator.transform.DOScale(_model.Range * 2f, 0.8f).SetEase(Ease.OutElastic, 1.2f, 1);
	}

	protected override void OnViewDestroy()
	{
		base.OnViewDestroy();
		if(_model != null)
		{
			_model.TargetSetEvent -= OnTargetSetEvent;
			_model = null;
		}
	}

	protected override void Update()
	{
		base.Update();

		if(_model != null)
		{
			_turretTurningPoint.transform.rotation = Quaternion.Euler(0, 0, _model.TurretNeckRotation);
		}
	}

	private void OnTargetSetEvent(EntityModel newTarget, EntityModel previousTarget)
	{
		//TODO: PLAY COOL SOUND AND SHIT
	}
}
