using System;

public class EntityModel : BaseModel, IModelTransformHolder
{
    public ModelTransform ModelTransform
    {
        get; private set;
    }

    public EntityModel()
    {
        ModelTransform = new ModelTransform();
        EntityTracker.Instance.Register(this);
    }
}