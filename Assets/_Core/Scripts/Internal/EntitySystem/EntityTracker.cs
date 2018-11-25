using System;

public class EntityTracker : ModelHolder<EntityModel>
{
	public delegate void EntityTagHandler(EntityModel entity, string tag);
	public delegate void EntityComponentHandler(EntityModel entity, BaseModelComponent component);
	public event EntityTagHandler EntityAddedTagEvent;
	public event EntityTagHandler EntityRemovedTagEvent;
	public event EntityComponentHandler EntityAddedComponentEvent;
	public event EntityComponentHandler EntityRemovedComponentEvent;

	public static EntityTracker Instance
	{
		get
		{
			if(_instance == null)
			{
				_instance = new EntityTracker();
			}

			return _instance;
		}
	}

	private static EntityTracker _instance;

	public void Register(EntityModel model)
	{
		if(Track(model))
		{
			if(model.IsDestroyed)
			{
				Unregister(model);
			}
			else
			{
				model.DestroyEvent += OnDestroyEvent;
				model.ModelTags.TagAddedEvent += OnTagAddedEvent;
				model.ModelTags.TagRemovedEvent += OnTagRemovedEvent;
				model.AddedComponentToModelEvent += OnComponentAddedEvent;
				model.RemovedComponentFromModelEvent += OnComponentRemovedEvent;
			}
		}
	}

	public void Unregister(EntityModel model)
	{
		if(Untrack(model))
		{
			model.DestroyEvent -= OnDestroyEvent;
			model.ModelTags.TagAddedEvent -= OnTagAddedEvent;
			model.ModelTags.TagRemovedEvent -= OnTagRemovedEvent;
			model.AddedComponentToModelEvent -= OnComponentAddedEvent;
			model.RemovedComponentFromModelEvent -= OnComponentRemovedEvent;
		}
	}

	private void OnDestroyEvent(BaseModel destroyedEntity)
	{
		Unregister((EntityModel)destroyedEntity);
	}

	private void OnTagAddedEvent(BaseModel entity, string tag)
	{
		if(EntityAddedTagEvent != null)
		{
			EntityAddedTagEvent((EntityModel)entity, tag);
		}
	}

	private void OnTagRemovedEvent(BaseModel entity, string tag)
	{
		if(EntityRemovedTagEvent != null)
		{
			EntityRemovedTagEvent((EntityModel)entity, tag);
		}
	}

	private void OnComponentAddedEvent(BaseModel entity, BaseModelComponent component)
	{
		if(EntityAddedComponentEvent != null)
		{
			EntityAddedComponentEvent((EntityModel)entity, component);
		}
	}

	private void OnComponentRemovedEvent(BaseModel entity, BaseModelComponent component)
	{
		if(EntityRemovedComponentEvent != null)
		{
			EntityRemovedComponentEvent((EntityModel)entity, component);
		}
	}
}