using UnityEngine;

public class WaveSystemView : MonoBaseView
{
	[SerializeField]
	private EnemyViewFactory _enemyViewFactory;

	private WaveSystemModel _waveSystemModel;

	#region LifeCycle

	protected override void OnViewReady()
	{
		_waveSystemModel = MVCUtil.GetModel<WaveSystemModel>(this);
		_waveSystemModel.SpawnEnemyEvent += OnSpawnEnemyEvent;
	}

	protected override void OnViewDestroy()
	{
		_waveSystemModel.SpawnEnemyEvent -= OnSpawnEnemyEvent;
		_waveSystemModel = null;
	}

	#endregion

	private void OnSpawnEnemyEvent(EnemyModel enemy)
	{
		_enemyViewFactory.CreateEnemyView(enemy);
	}

}
