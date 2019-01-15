using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraModel : EntityModel
{
	public float MaxOrtographicSize
	{
		get; private set;
	}

	public float OrthographicSize
	{
		get; private set;
	}

	public CameraModel(float maxOrtographicSize, float ortographicSize)
	{
		MaxOrtographicSize = maxOrtographicSize;
		OrthographicSize = ortographicSize;
	}

	public void SetOrtographicSize(float ortographicSize)
	{
		OrthographicSize = Mathf.Clamp(ortographicSize, 0, MaxOrtographicSize);
	}
}
