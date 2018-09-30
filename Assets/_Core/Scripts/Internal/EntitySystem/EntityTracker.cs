public class EntityTracker : ModelHolder<EntityModel>
{
    public delegate void EntityTagHandler(EntityModel entity, string tag);
    public event EntityTagHandler EntityAddedTagEvent;
    public event EntityTagHandler EntityRemovedTagEvent;

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
            }
        }
    }

    public void Unregister(EntityModel model)
    {
        if (Untrack(model))
        {
            model.DestroyEvent -= OnDestroyEvent;
            model.ModelTags.TagAddedEvent -= OnTagAddedEvent;
            model.ModelTags.TagRemovedEvent -= OnTagRemovedEvent;
        }
    }

    private void OnDestroyEvent(BaseModel destroyedEntity)
    {
        Unregister((EntityModel)destroyedEntity);
    }

    private void OnTagAddedEvent(BaseModel entity, string tag)
    {
        if (EntityAddedTagEvent != null)
        {
            EntityAddedTagEvent((EntityModel)entity, tag);
        }
    }

    private void OnTagRemovedEvent(BaseModel entity, string tag)
    {
        if (EntityRemovedTagEvent != null)
        {
            EntityRemovedTagEvent((EntityModel)entity, tag);
        }
    }
}

public enum TagFilterType
{
    None,
    HasAnyTag,
    HasAllTags,
}

public class EntityFilter<T> : ModelHolder<T> where T : EntityModel
{
    public TagFilterType FilterType { get; private set; }
    public string[] FilterTags { get; private set; }

    public EntityFilter()
    {
        FilterType = TagFilterType.None;
        FilterTags = new string[] { };

        InternalSetup();
    }

    public EntityFilter(TagFilterType filterType, params string[] tags)
    {
        FilterType = filterType;
        FilterTags = tags;
        InternalSetup();
    }

    public override void Clean()
    {
        EntityTracker.Instance.EntityAddedTagEvent -= OnEntityAddedTagEvent;
        EntityTracker.Instance.EntityRemovedTagEvent -= OnEntityRemovedTagEvent;
        EntityTracker.Instance.TrackedEvent -= OnTrackedEvent;
        EntityTracker.Instance.UntrackedEvent -= OnEntityUntrackedEvent;
        base.Clean();
    }

    private void OnTrackedEvent(EntityModel entity)
    {
        T e = entity as T;
        if (e != null && HasFilterPermission(e))
        {
            Track(e);
        }
    }

    private void OnEntityRemovedTagEvent(EntityModel entity, string tag)
    {
        T e = entity as T;
        if (e != null && !HasFilterPermission(e))
        {
            Untrack(e);
        }
    }

    private void OnEntityAddedTagEvent(EntityModel entity, string tag)
    {
        OnTrackedEvent(entity);
    }

    private void OnEntityUntrackedEvent(EntityModel entity)
    {
        T e = entity as T;
        if (e != null)
        {
            Untrack(e);
        }
    }

    private void InternalSetup()
    {
        EntityTracker.Instance.EntityAddedTagEvent += OnEntityAddedTagEvent;
        EntityTracker.Instance.EntityRemovedTagEvent += OnEntityRemovedTagEvent;
        EntityTracker.Instance.TrackedEvent += OnTrackedEvent;
        EntityTracker.Instance.UntrackedEvent += OnEntityUntrackedEvent;
        FillWithAlreadyExistingMatches();
    }

    private void FillWithAlreadyExistingMatches()
    {
        System.Collections.ObjectModel.ReadOnlyCollection<T> t = EntityTracker.Instance.GetAll<T>(HasFilterPermission);

        for (int i = 0; i < t.Count; i++)
        {
            Track(t[i]);
        }
    }

    public bool HasFilterPermission(T entity)
    {
        switch(FilterType)
        {
            case TagFilterType.HasAnyTag:
                return entity.ModelTags.HasAnyTag(FilterTags);
            case TagFilterType.HasAllTags:
                return entity.ModelTags.HasAllTags(FilterTags);
            default:
                return true;
        }
    }
}