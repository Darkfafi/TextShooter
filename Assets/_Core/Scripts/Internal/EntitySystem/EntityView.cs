using UnityEngine;

public class EntityView : MonoBaseView
{
    public EntityModel SelfModel { get; private set; }
    protected EntityTransform ViewDeltaTransform { get; private set; }

    [SerializeField]
    protected bool IgnoreModelTransform = false;

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
            Vector3 p = transform.position;
            Vector3 r = transform.rotation.eulerAngles;
            Vector3 s = transform.localScale;

            if(!IgnoreModelTransform)
            {
                p = SelfModel.Transform.Position;
                r = SelfModel.Transform.Rotation;
                s = SelfModel.Transform.Scale;
            }

            p += ViewDeltaTransform.Position;
            r += ViewDeltaTransform.Rotation;
            s += ViewDeltaTransform.Scale;


            transform.position = p;
            transform.rotation = Quaternion.Euler(r);
            transform.localScale = s;
        }
    }
}
