using UnityEngine;

public class EntityView : MonoBaseView
{
    public EntityModel SelfModel { get; private set; }
    public ModelTransform ViewDeltaTransform { get; private set; }

    protected bool IgnoreModelTransform = false;

    //TODO: IDEA: Make debug menu to set the position of the model through the view to make it easier to debug through the editor

    protected override void OnViewReady()
    {
        ViewDeltaTransform = new ModelTransform();
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
        if (ViewDeltaTransform != null && SelfModel != null && !SelfModel.IsDestroyed && !IgnoreModelTransform)
        {
            Vector3 p = SelfModel.ModelTransform.Position + ViewDeltaTransform.Position;
            Vector3 r = SelfModel.ModelTransform.Rotation + ViewDeltaTransform.Rotation;
            Vector3 s = SelfModel.ModelTransform.Scale + ViewDeltaTransform.Scale;

            transform.position = p;
            transform.rotation = Quaternion.Euler(r);
            transform.localScale = s;
        }
    }
}
