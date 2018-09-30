public class EntityTracker : ModelHolder<EntityModel>
{
    public event EntityModel.EntityTagHandler EntityAddedTagEvent;
    public event EntityModel.EntityTagHandler EntityRemovedTagEvent;

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
                model.TagAddedEvent += OnTagAddedEvent;
                model.TagRemovedEvent += OnTagRemovedEvent;
            }
        }
    }

    public void Unregister(EntityModel model)
    {
        if (Untrack(model))
        {
            model.DestroyEvent -= OnDestroyEvent;
            model.TagAddedEvent -= OnTagAddedEvent;
            model.TagRemovedEvent -= OnTagRemovedEvent;
        }
    }

    private void OnDestroyEvent(BaseModel destroyedEntity)
    {
        Unregister((EntityModel)destroyedEntity);
    }

    private void OnTagAddedEvent(EntityModel entity, string tag)
    {
        EntityAddedTagEvent(entity, tag);
    }

    private void OnTagRemovedEvent(EntityModel entity, string tag)
    {
        EntityRemovedTagEvent(entity, tag);
    }
}

public class EntityFilter<T> : ModelHolder<T> where T : EntityModel
{
    public enum FilterType
    {
        None,
        HasAnyTag,
        HasAllTags,
    }

    public FilterType UsingFilterType { get; private set; }
    public string[] FilterTags { get; private set; }

    public EntityFilter()
    {
        UsingFilterType = FilterType.None;
        FilterTags = new string[] { };

        InternalSetup();
    }

    public EntityFilter(FilterType filterType, params string[] tags)
    {
        UsingFilterType = filterType;
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
        switch(UsingFilterType)
        {
            case FilterType.HasAnyTag:
                return entity.HasAnyTag(FilterTags);
            case FilterType.HasAllTags:
                return entity.HasAllTags(FilterTags);
            default:
                return true;
        }
    }
}