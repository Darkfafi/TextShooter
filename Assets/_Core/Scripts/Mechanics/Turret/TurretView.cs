using System;
using UnityEngine;

public class TurretView : EntityView<TurretModel>
{
    [SerializeField]
    private GameObject _turretTurningPoint;

    public override bool LinkModel(IModel model)
    {
        if(Model != null)
        {
            Model.TargetSetEvent -= OnTargetSetEvent;
        }

        if(base.LinkModel(model))
        {
            Model.TargetSetEvent += OnTargetSetEvent;
            return true;
        }

        return false;
    }

    protected void Update()
    {
        TurretFocus();
    }

    private void TurretFocus()
    {
        if(Model.CurrentTarget == null)
        {
            return;
        }

        MonoBehaviour targetView1 = Model.CurrentTarget.Controller.CoreView as MonoBehaviour;
        EntityView targetView = MVCUtil.GetView<EntityView>(Model.CurrentTarget);

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
