using UnityEngine;

public class TimekeeperView : MonoBaseView
{
    private TimekeeperModel _model;

    protected override void OnViewReady()
    {
        _model = MVCUtil.GetModel<TimekeeperModel>(this);
    }

    protected void Update()
    {
        if (_model != null)
        {
            _model.FrameTick(Time.unscaledDeltaTime);
        }
    }

    protected override void OnViewDestroy()
    {
        _model = null;
    }
}
