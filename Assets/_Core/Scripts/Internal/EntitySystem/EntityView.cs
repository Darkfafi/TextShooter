using UnityEngine;

public class EntityView : MonoBaseView
{
    public EntityModel SelfModel { get; private set; }
    protected EntityTransform ViewDeltaTransform { get; private set; }

    protected override void OnViewReady()
    {
        ViewDeltaTransform = new EntityTransform();
        ViewDeltaTransform.Scale = Vector3.zero;

        SelfModel = MVCUtil.GetModel<EntityModel>(this);
    }

    protected override void OnViewDestroy()
    {
        ViewDeltaTransform = null;
        SelfModel = null;
    }

    protected virtual void Update()
    {
        if (ViewDeltaTransform != null && SelfModel != null)
        {
            transform.position = SelfModel.Transform.Position + ViewDeltaTransform.Position;
            transform.rotation = Quaternion.Euler(SelfModel.Transform.Rotation + ViewDeltaTransform.Rotation);
            transform.localScale = SelfModel.Transform.Scale + ViewDeltaTransform.Scale;
        }
    }
}
