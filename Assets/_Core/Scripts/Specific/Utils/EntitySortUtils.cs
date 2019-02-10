using System;
using UnityEngine;

public static class EntitySortUtils
{
	public static Comparison<EntityModel> SortOnClosestTo(this Vector3 location)
	{
		return (a, b) =>
		{
			float distA = (a.ModelTransform.Position - location).magnitude;
			float distB = (b.ModelTransform.Position - location).magnitude;
			return (int)(distA - distB);
		};
	}

	public static Comparison<EntityModel> SortOnClosestTo(this EntityModel entity)
	{
		return SortOnClosestTo(entity.ModelTransform.Position);
	}

}
