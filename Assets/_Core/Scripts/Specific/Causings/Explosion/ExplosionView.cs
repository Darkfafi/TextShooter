using System;
using DG.Tweening;

public class ExplosionView : EntityView
{
	private Action _explosionDestroyPermissionSignaller;

	protected override void OnPreViewReady()
	{
		LinkingController.MethodPermitter.BlockPermission(ExplosionModel.EXPLOSION_DESTROY_PERMISSION, out _explosionDestroyPermissionSignaller);
	}

	protected override void OnViewReady()
	{
		base.OnViewReady();
		IgnoreModelTransform = true;
		transform.localScale = new UnityEngine.Vector3(1, 0, 1);
		transform.DOScaleY(1, 0.5f).SetEase(Ease.OutBack);
		transform.DOShakeRotation(0.7f, 5).OnComplete(Signal);
	}

	private void Signal()
	{
		_explosionDestroyPermissionSignaller();
	}

	protected override void OnViewDestroy()
	{
		_explosionDestroyPermissionSignaller = null;
		base.OnViewDestroy();
	}
}
