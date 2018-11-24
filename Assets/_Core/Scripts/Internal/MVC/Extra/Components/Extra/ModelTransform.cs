using UnityEngine;

public class ModelTransform : BaseModelComponent
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

	public void SetPos(Vector3 position)
	{
		Position = position;
	}

    public void SetPosX(float x)
    {
        Vector3 p = Position;
        p.x = x;
		SetPos(p);
	}

    public void SetPosY(float y)
    {
        Vector3 p = Position;
        p.y = y;
		SetPos(p);
	}

    public void SetPosZ(float z)
    {
        Vector3 p = Position;
        p.z = z;
		SetPos(p);
	}

    public void TranslateRotation(float x, float y, float z)
    {
        Vector3 r = Rotation;
        r.x += x;
        r.y += y;
        r.z += z;
        Rotation = r;
    }

    public void SetRotX(float x)
    {
        Vector3 r = Rotation;
        r.x = x;
        Rotation = r;
    }

    public void SetRotY(float y)
    {
        Vector3 r = Rotation;
        r.y = y;
        Rotation = r;
    }

    public void SetRotZ(float z)
    {
        Vector3 r = Rotation;
        r.z = z;
        Rotation = r;
    }
}