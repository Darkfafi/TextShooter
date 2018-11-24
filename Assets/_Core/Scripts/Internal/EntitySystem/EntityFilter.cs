using System;
using System.Collections.Generic;

public enum TagFilterType
{
    None,
    HasAnyTag,
    HasAllTags,
}

public class EntityFilter<T> : ModelHolder<T> where T : EntityModel
{
    public Filter Filter { get; private set; }

	#region Static Construction

	private static List<EntityFilter<T>> _cachedFilters = new List<EntityFilter<T>>();
    private static Dictionary<EntityFilter<T>, int> _cachedFiltersReferenceCounter = new Dictionary<EntityFilter<T>, int>();

    public static EntityFilter<T> Create()
    {
		Filter myFilter;
		Filter.OpenFilterCreation();
		Filter.CreateTagsFilter();
		Filter.CloseFilterCreation(out myFilter);
        return Create(myFilter);
    }

	public static EntityFilter<T> Create(Filter filter)
    {
        for (int i = _cachedFilters.Count - 1; i >= 0; i--)
        {
            if (_cachedFilters[i].Filter.Equals(filter))
            {
                AddReference(_cachedFilters[i]);
                return _cachedFilters[i];
            }
        }

        EntityFilter<T> self = new EntityFilter<T>(filter);
        AddReference(self);
        _cachedFilters.Add(self);
        return self;
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

    private EntityFilter(Filter filter)
    {
		Filter = filter;
		EntityTracker.Instance.EntityAddedTagEvent += OnEntityAddedTagEvent;
        EntityTracker.Instance.EntityRemovedTagEvent += OnEntityRemovedTagEvent;
		EntityTracker.Instance.EntityAddedComponentEvent += OnEntityAddedComponentEvent;
		EntityTracker.Instance.EntityRemovedComponentEvent += OnEntityRemovedComponentEvent;
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
			EntityTracker.Instance.EntityAddedComponentEvent -= OnEntityAddedComponentEvent;
			EntityTracker.Instance.EntityRemovedComponentEvent -= OnEntityRemovedComponentEvent;
			EntityTracker.Instance.TrackedEvent -= OnTrackedEvent;
            EntityTracker.Instance.UntrackedEvent -= OnEntityUntrackedEvent;
            base.Clean();
        }
    }

	public bool Equals(EntityFilter<T> filter)
    {
        return Equals(filter.Filter);
    }

    private void OnTrackedEvent(EntityModel entity)
    {
        T e = entity as T;
        if (e != null && Filter.HasFilterPermission(e))
        {
            Track(e);
        }
	}

	private void OnEntityAddedComponentEvent(EntityModel entity, BaseModelComponent component)
	{
		TrackLogics(entity);
	}

	private void OnEntityRemovedComponentEvent(EntityModel entity, BaseModelComponent component)
	{
		TrackLogics(entity);
	}

	private void OnEntityAddedTagEvent(EntityModel entity, string tag)
	{
		TrackLogics(entity);
	}

	private void OnEntityRemovedTagEvent(EntityModel entity, string tag)
    {
		TrackLogics(entity);
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
        T[] t = EntityTracker.Instance.GetAll<T>(Filter.HasFilterPermission);
        for (int i = 0; i < t.Length; i++)
        {
            Track(t[i]);
        }
    }

	private void TrackLogics(EntityModel entity)
	{
		T e = entity as T;
		if (e != null)
		{
			if (Filter.HasFilterPermission(e))
			{
				Track(e);
			}
			else
			{
				Untrack(e);
			}
		}
	}
}

public class Filter
{
	public TagFilterType FilterType { get; private set; }

	public string[] FilterTags
	{
		get
		{
			return _filterTags.ToArray();
		}
	}

	public Type[] FilterComponents
	{
		get
		{
			return _componentsToFilterOn.ToArray();
		}
	}

	private List<string> _filterTags = new List<string>();
	private List<Type> _componentsToFilterOn = new List<Type>();

	private static bool _filterOpened = false;
	private static Filter _constructingFiltersParameters;

	public static void OpenFilterCreation()
	{
		_filterOpened = true;
	}

	public static void CreateTagsFilter()
	{
		_constructingFiltersParameters = new Filter(new string[] { }, TagFilterType.None);
	}

	public static void CreateHasAnyTagsFilter(params string[] tags)
	{
		_constructingFiltersParameters = new Filter(tags, TagFilterType.HasAnyTag);
	}

	public static void CreateHasAllTagsFilter(params string[] tags)
	{
		_constructingFiltersParameters = new Filter(tags, TagFilterType.HasAllTags);
	}

	public static void AddComponentToFilterOn<T>() where T : BaseModelComponent
	{
		Type t = typeof(T);
		if (!_constructingFiltersParameters._componentsToFilterOn.Contains(t))
		{
			_constructingFiltersParameters._componentsToFilterOn.Add(t);
		}
	}

	public static void CloseFilterCreation(out Filter filterCreated)
	{
		filterCreated = _constructingFiltersParameters;
		_filterOpened = false;
		_constructingFiltersParameters = null;
	}

	public bool HasFilterPermission(EntityModel entity)
	{
		bool hasPermission = false;

		switch (FilterType)
		{
			case TagFilterType.HasAnyTag:
				hasPermission = entity.ModelTags.HasAnyTag(FilterTags);
				break;
			case TagFilterType.HasAllTags:
				hasPermission = entity.ModelTags.HasAllTags(FilterTags);
				break;
			default:
				hasPermission = true;
				break;
		}

		if(!hasPermission)
		{
			return false;
		}

		for(int i = 0, c = _componentsToFilterOn.Count; i < c; i++)
		{
			if(!entity.HasComponent(_componentsToFilterOn[i]))
			{
				return false;
			}
		}

		return true;
	}

	public bool Equals(Filter filter)
	{
		if (FilterType == filter.FilterType && FilterTags.Length == filter.FilterTags.Length && FilterComponents.Length == filter.FilterComponents.Length)
		{
			for (int i = 0, c = filter.FilterTags.Length; i < c; i++)
			{
				if (!_filterTags.Contains(filter.FilterTags[i]))
				{
					return false;
				}
			}

			for (int i = 0, c = filter.FilterComponents.Length; i < c; i++)
			{
				if (!_componentsToFilterOn.Contains(filter.FilterComponents[i]))
				{
					return false;
				}
			}

			return true;
		}

		return false;
	}

	private Filter(string[] tags, TagFilterType tagFilterType)
	{
		if (_filterOpened)
		{
			_filterTags = new List<string>(tags);
			FilterType = tagFilterType;
		}
		else
		{
			throw new Exception("Tried creating a filter without the call being between an `OpenFilterCreation` and `CloseFilterCreation` call");
		}
	}
}