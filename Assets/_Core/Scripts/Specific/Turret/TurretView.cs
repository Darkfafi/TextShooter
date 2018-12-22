using UnityEngine;
using DG.Tweening;

public class TurretView : EntityView
{
	[SerializeField]
	private GameObject _turretTurningPoint;

	[SerializeField]
	private GameObject _rangeIndicator;

	[SerializeField]
	private GameObject _muzzleFlareObject;

	[SerializeField]
	private Transform _muzzleFlareLocation;

	private TurretModel _model;

	protected override void OnViewReady()
	{
		base.OnViewReady();
		_model = MVCUtil.GetModel<TurretModel>(this);
		DisableFlare();
		_model.GunFiredEvent += OnGunFiredEvent;
		_model.TargetSetEvent += OnTargetSetEvent;
		_model.RangeChangedEvent += OnRangeChangedEvent;
		_model.GunActiveStateChangedEvent += OnGunActiveStateChangedEvent;
	}

	protected override void OnViewDestroy()
	{
		base.OnViewDestroy();
		if(_model != null)
		{
			_model.GunFiredEvent -= OnGunFiredEvent;
			_model.TargetSetEvent -= OnTargetSetEvent;
			_model.RangeChangedEvent -= OnRangeChangedEvent;
			_model.GunActiveStateChangedEvent -= OnGunActiveStateChangedEvent;
			_model = null;
		}
	}

	private void OnGunFiredEvent()
	{
		_muzzleFlareObject.SetActive(true);
		_muzzleFlareObject.transform.transform.position = _muzzleFlareLocation.position;
		_muzzleFlareObject.transform.rotation = _muzzleFlareLocation.rotation;
		CancelInvoke("DisableFlare");
		Invoke("DisableFlare", 0.05f);
	}

	private void OnGunActiveStateChangedEvent(bool state)
	{
		VisualizeRange(state ? _model.Range : 0f);
	}

	private void OnRangeChangedEvent(float range)
	{
		VisualizeRange(range);
	}

	private void DisableFlare()
	{
		_muzzleFlareObject.SetActive(false);
	}

	protected override void Update()
	{
		base.Update();

		if(_model != null)
		{
			_turretTurningPoint.transform.rotation = Quaternion.Euler(0, 0, _model.TurretNeckRotation);
		}
	}

	private void VisualizeRange(float range)
	{
		_rangeIndicator.transform.DOScale(range * 2f, 0.8f).SetEase(Ease.OutElastic, 1.2f, 1);
	}

	private void OnTargetSetEvent(EntityModel newTarget, EntityModel previousTarget)
	{
		//TODO: PLAY COOL SOUND AND SHIT
	}
}
