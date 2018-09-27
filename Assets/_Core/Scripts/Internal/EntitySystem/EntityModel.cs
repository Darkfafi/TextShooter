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


    public void TranslatePosition(float x, float y, float z)
    {
        Vector3 p = Position;
        p.x += x;
        p.y += y;
        p.z += z;
        Position = p;
    }

    public void SetPosX(float x)
    {
        Vector3 p = Position;
        p.x = x;
        Position = p;
    }

    public void SetPosY(float y)
    {
        Vector3 p = Position;
        p.y = y;
        Position = p;
    }

    public void SetPosZ(float z)
    {
        Vector3 p = Position;
        p.z = z;
        Position = p;
    }
}