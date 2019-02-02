using UnityEngine;

public class ExplosionViewFactory : BaseMonoViewFactory<ExplosionModel, ExplosionView>
{
	[SerializeField]
	private ExplosionView _explosionViewPrefab;

	protected override ExplosionView GetViewPrefabForModel(ExplosionModel model)
	{
		return _explosionViewPrefab;
	}
}
