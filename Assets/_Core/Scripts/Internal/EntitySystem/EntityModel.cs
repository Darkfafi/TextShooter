using System;

public class EntityModel : BaseModel
{
	public EntityModel()
	{
		ModelTransform = AddComponent<ModelTransform>();
		ModelTags = AddComponent<ModelTags>();
		EntityTracker.Instance.Register(this);
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

		return true;
	}

	protected override void OnModelDestroy()
	{
		base.OnModelDestroy();
		ModelTags = null;
		ModelTransform = null;
	}
}