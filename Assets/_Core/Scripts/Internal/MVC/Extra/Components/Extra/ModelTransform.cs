using UnityEngine;

public class ModelTransform : BaseModelComponent
{
	public Vector3 Position
	{
		get; private set;
	}

	public Vector3 Rotation
	{
		get; private set;
	}

	public Vector3 Scale
	{
		get; private set;
	}

	public ModelTransform()
	{
		Position = Vector3.zero;
		Rotation = Vector3.zero;
		Scale = Vector3.one;
	}

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

	public void SetRot(Vector3 rotation)
	{
		Rotation = rotation;
	}

	public void TranslateRotation(float x, float y, float z)
	{
		Vector3 r = Rotation;
		r.x += x;
		r.y += y;
		r.z += z;
		SetRot(r);
	}

	public void SetRotX(float x)
	{
		Vector3 r = Rotation;
		r.x = x;
		SetRot(r);
	}

	public void SetRotY(float y)
	{
		Vector3 r = Rotation;
		r.y = y;
		SetRot(r);
	}

	public void SetRotZ(float z)
	{
		Vector3 r = Rotation;
		r.z = z;
		SetRot(r);
	}

	public void SetScale(Vector3 scale)
	{
		Scale = scale;
	}

	public void SetScaleX(float x)
	{
		Vector3 s = Scale;
		s.x = x;
		SetScale(s);
	}

	public void SetScaleY(float y)
	{
		Vector3 s = Scale;
		s.y = y;
		SetScale(s);
	}

	public void SetScaleZ(float z)
	{
		Vector3 s = Scale;
		s.z = z;
		SetScale(s);
	}
}