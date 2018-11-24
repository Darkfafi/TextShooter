using System.Collections.Generic;

public enum TagFilterType
{
    None,
    HasAnyTag,
    HasAllTags,
}

public class EntityFilter<T> : ModelHolder<T> where T : EntityModel
{
    public TagFilterType FilterType { get; private set; }

    public string[] FilterTags
    {
        get
        {
            return _filterTagsList.ToArray();
        }
    }

    private List<string> _filterTagsList;

    #region Static Construction

    private static List<EntityFilter<T>> _cachedFilters = new List<EntityFilter<T>>();
    private static Dictionary<EntityFilter<T>, int> _cachedFiltersReferenceCounter = new Dictionary<EntityFilter<T>, int>();

    public static EntityFilter<T> Create()
    {
        return Create(TagFilterType.None, new string[] { });
    }

    public static EntityFilter<T> Create(params string[] tags)
    {
        return Create(TagFilterType.HasAnyTag, tags);
    }

    public static EntityFilter<T> Create(TagFilterType filterType, params string[] tags)
    {
        for (int i = _cachedFilters.Count - 1; i >= 0; i--)
        {
            if (_cachedFilters[i].Equals(filterType, tags))
            {
                AddReference(_cachedFilters[i]);
                return _cachedFilters[i];
            }
        }

        EntityFilter<T> filter = new EntityFilter<T>(filterType, tags);
        AddReference(filter);
        _cachedFilters.Add(filter);
        return filter;
    }

    private static void AddReference(EntityFilter<T> instance)
    {
        if(HasReferences(instance))
        {
            _cachedFiltersReferenceCounter[instance]++;
        }
        else
        {
            _cachedFiltersReferenceCounter.Add(instance, 1);
        }
    }

    private static bool HasReferences(EntityFilter<T> instance)
    {
        return _cachedFiltersReferenceCounter.ContainsKey(instance);
    }

    private static void RemoveReference(EntityFilter<T> instance)
    {
        bool remove = false;
        if (HasReferences(instance))
        {
            _cachedFiltersReferenceCounter[instance]--;
            if(_cachedFiltersReferenceCounter[instance] == 0)
            {
                _cachedFiltersReferenceCounter.Remove(instance);
                remove = true;
            }
        }
        else
        {
            remove = true;
        }

        if (remove)
        {
            _cachedFilters.Remove(instance);
        }
    }

    #endregion

    private EntityFilter(TagFilterType filterType, params string[] tags)
    {
        FilterType = filterType;
        _filterTagsList = new List<string>(tags);
        EntityTracker.Instance.EntityAddedTagEvent += OnEntityAddedTagEvent;
        EntityTracker.Instance.EntityRemovedTagEvent += OnEntityRemovedTagEvent;
        EntityTracker.Instance.TrackedEvent += OnTrackedEvent;
        EntityTracker.Instance.UntrackedEvent += OnEntityUntrackedEvent;
        FillWithAlreadyExistingMatches();
    }

    public override void Clean()
    {
        RemoveReference(this);
        if (!HasReferences(this))
        {
            EntityTracker.Instance.EntityAddedTagEvent -= OnEntityAddedTagEvent;
            EntityTracker.Instance.EntityRemovedTagEvent -= OnEntityRemovedTagEvent;
            EntityTracker.Instance.TrackedEvent -= OnTrackedEvent;
            EntityTracker.Instance.UntrackedEvent -= OnEntityUntrackedEvent;
            base.Clean();
        }
    }

    public bool HasFilterPermission(T entity)
    {
        switch (FilterType)
        {
            case TagFilterType.HasAnyTag:
                return entity.ModelTags.HasAnyTag(FilterTags);
            case TagFilterType.HasAllTags:
                return entity.ModelTags.HasAllTags(FilterTags);
            default:
                return true;
        }
    }

    public bool Equals(EntityFilter<T> filter)
    {
        return Equals(filter.FilterType, filter.FilterTags);
    }

    public bool Equals(TagFilterType filterType, string[] tags)
    {
        if (FilterType == filterType && FilterTags.Length == tags.Length)
        {
            for (int i = 0, c = tags.Length; i < c; i++)
            {
                if (!_filterTagsList.Contains(tags[i]))
                {
                    return false;
                }
            }

            return true;
        }

        return false;
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

    private void FillWithAlreadyExistingMatches()
    {
        T[] t = EntityTracker.Instance.GetAll<T>(HasFilterPermission);
        for (int i = 0; i < t.Length; i++)
        {
            Track(t[i]);
        }
    }
}