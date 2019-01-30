using System;
using UnityEngine;

public class EntityModel : BaseModel
{
	public EntityModel()
	{
		Initialize(Vector3.zero, Vector3.zero, Vector3.one);
	}

	public EntityModel(Vector3 position)
	{
		Initialize(position, Vector3.zero, Vector3.one);
	}

	public EntityModel(Vector3 position, Vector3 rotation)
	{
		Initialize(position, rotation, Vector3.one);
	}

	public EntityModel(Vector3 position, Vector3 rotation, Vector3 scale)
	{
		Initialize(position, rotation, scale);
	}

	public ModelTags ModelTags
	{
		get; private set;
	}

	public ModelTransform ModelTransform
	{
		get; private set;
	}

	protected override bool ComponentActionValidation(ModelComponents.ModelComponentsAction action, Type componentType)
	{
		if(componentType == typeof(ModelTransform))
		{
			if(action == ModelComponents.ModelComponentsAction.RemoveComponent)
			{
				return IsDestroyed;
			}

			if(action == ModelComponents.ModelComponentsAction.AddComponent)
			{
				return ModelTransform == null;
			}
		}

		if(componentType == typeof(ModelTags))
		{
			if(action == ModelComponents.ModelComponentsAction.RemoveComponent)
			{
				return IsDestroyed;
			}

			if(action == ModelComponents.ModelComponentsAction.AddComponent)
			{
				return ModelTags == null;
			}
		}

		return true;
	}

	protected override void OnModelDestroy()
	{
		base.OnModelDestroy();
		ModelTags = null;
		ModelTransform = null;
	}

	private void Initialize(Vector3 position, Vector3 rotation, Vector3 scale)
	{
		ModelTransform = AddComponent<ModelTransform>();
		ModelTags = AddComponent<ModelTags>();

		ModelTransform.SetPos(position);
		ModelTransform.SetRot(rotation);
		ModelTransform.SetScale(scale);

		EntityTracker.Instance.Register(this);
	}
}