using UnityEngine;

public class TurretView : EntityView
{
    [SerializeField]
    private GameObject _turretTurningPoint;

    private TurretModel _model;

    protected override void OnViewReady()
    {
        _model = MVCUtil.GetModel<TurretModel>(this);
        _model.TargetSetEvent += OnTargetSetEvent;
    }

    public override void DestroyView()
    {
        if (_model != null)
        {
            _model.TargetSetEvent -= OnTargetSetEvent;
            _model = null;
        }
    }

    protected void Update()
    {
        if(_model != null)
            TurretFocus();
    }

    private void TurretFocus()
    {
        if(_model.CurrentTarget == null)
        {
            return;
        }
        
        EntityView targetView = MVCUtil.GetView<EntityView>(_model.CurrentTarget);

        float x = targetView.transform.position.x - _turretTurningPoint.transform.position.x;
        float y = targetView.transform.position.y - _turretTurningPoint.transform.position.y;

        float angle = (Mathf.Atan2(y, x) * Mathf.Rad2Deg) - 90f;

        float newAngle = Mathf.LerpAngle(_turretTurningPoint.transform.eulerAngles.z, angle, Time.deltaTime * 7.4f);

        _turretTurningPoint.transform.rotation = Quaternion.Euler(0, 0, newAngle);
    }

    private void OnTargetSetEvent(EntityModel newTarget, EntityModel previousTarget)
    {
        //TODO: PLAY COOL SOUND AND SHIT
    }
}
