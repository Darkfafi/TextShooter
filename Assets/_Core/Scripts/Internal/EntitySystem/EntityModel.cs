using UnityEngine;

public class EntityModel : BaseModel
{
    public EntityTransform Transform { get; protected set; }

    public EntityModel()
    {
        Transform = new EntityTransform();
    }
}

public class EntityTransform
{
    public Vector3 Position;
    public Vector3 Rotation;
    public Vector3 Scale;
}