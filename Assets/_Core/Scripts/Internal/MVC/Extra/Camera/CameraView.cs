using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraView : EntityView
{
    public Camera Camera { get; private set; }
    private CameraModel _cameraModel;

    protected void Awake()
    {
        Camera = gameObject.GetComponent<Camera>();
    }

    protected override void Update()
    {
        if(_cameraModel == null)
        {
            return;
        }

        base.Update();
        Camera.orthographicSize = _cameraModel.OrthographicSize;
    }

    protected override void OnViewReady()
    {
        base.OnViewReady();
        _cameraModel = SelfModel as CameraModel;
        ViewDeltaTransform.SetPosZ(-10); // Make it so the unity camera is at the right distance without affecting the model data.
    }

    protected override void OnViewDestroy()
    {
        base.OnViewDestroy();
        Camera = null;
        _cameraModel = null;
    }
}
