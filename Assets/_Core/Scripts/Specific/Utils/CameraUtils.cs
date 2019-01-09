using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CameraUtils
{
	public enum Side
	{
		Top, Right, Bottom, Left, Any
	}

	public static Vector2 GetOutOfMaxOrthographicLocation(this CameraModel camera, Side side, float marginOutsideCamera = 1f)
	{
		float distY = camera.MaxOrtographicSize + marginOutsideCamera;
		float distX = distY * Screen.width / Screen.height;

		bool fullX;

		if(side == Side.Any)
			fullX = Random.value > 0.5f;
		else
			fullX = (side == Side.Left || side == Side.Right);

		int xMult = Random.value > 0.5f ? 1 : -1;
		int yMult = Random.value > 0.5f ? 1 : -1;

		if(side == Side.Right || side == Side.Left)
			xMult = side == Side.Right ? 1 : -1;

		if(side == Side.Top || side == Side.Bottom)
			yMult = side == Side.Top ? 1 : -1;

		float x = ((fullX) ? 1 : Random.value);
		float y = ((!fullX) ? 1 : Random.value);
		x = (Mathf.Lerp(0, distX, x)) * xMult;
		y = (Mathf.Lerp(0, distY, y)) * yMult;

		return new Vector2(x, y);
	}
}
