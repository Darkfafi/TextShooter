using UnityEngine;
public class WaveSystemView : MonoBaseView
{
	//private WaveSystemModel _waveSystemModel;

	#region LifeCycle

	protected override void OnViewReady()
	{
		//_waveSystemModel = MVCUtil.GetModel<WaveSystemModel>(this);
	}

	protected override void OnViewDestroy()
	{
		//_waveSystemModel = null;
	}

	#endregion
}
