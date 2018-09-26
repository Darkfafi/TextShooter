using UnityEngine;

public class EntityModel : BaseModel
{
    public EntityTransform Transform { get; protected set; }

    public EntityModel()
    {
        Transform = new EntityTransform();
        EntityTracker.Instance.Register(this);
    }
}

public class EntityTransform
{
    public Vector3 Position = Vector3.zero;
    public Vector3 Rotation = Vector3.zero;
    public Vector3 Scale = Vector3.one;
}