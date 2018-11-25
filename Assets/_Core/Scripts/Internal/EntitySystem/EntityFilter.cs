using System.Collections.Generic;

public class EntityFilter<T> : ModelHolder<T> where T : EntityModel
{
	public FilterRules FilterRules
	{
		get; private set;
	}

	#region Static Construction

	private static List<EntityFilter<T>> _cachedFilters = new List<EntityFilter<T>>();
	private static Dictionary<EntityFilter<T>, int> _cachedFiltersReferenceCounter = new Dictionary<EntityFilter<T>, int>();

	public static EntityFilter<T> Create()
	{
		return Create(FilterRules.CreateNoTagsFilter());
	}

	public static EntityFilter<T> Create(FilterRules filterRules)
	{
		for(int i = _cachedFilters.Count - 1; i >= 0; i--)
		{
			if(_cachedFilters[i].FilterRules.Equals(filterRules))
			{
				AddReference(_cachedFilters[i]);
				return _cachedFilters[i];
			}
		}

		EntityFilter<T> self = new EntityFilter<T>(filterRules);
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
		if(HasReferences(instance))
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

		if(remove)
		{
			_cachedFilters.Remove(instance);
		}
	}

	#endregion

	private EntityFilter(FilterRules filter)
	{
		FilterRules = filter;
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
		if(!HasReferences(this))
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
		return Equals(filter.FilterRules);
	}

	private void OnTrackedEvent(EntityModel entity)
	{
		T e = entity as T;
		if(e != null && FilterRules.HasFilterPermission(e))
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
		if(e != null)
		{
			Untrack(e);
		}
	}

	private void FillWithAlreadyExistingMatches()
	{
		T[] t = EntityTracker.Instance.GetAll<T>(FilterRules.HasFilterPermission);
		for(int i = 0; i < t.Length; i++)
		{
			Track(t[i]);
		}
	}

	private void TrackLogics(EntityModel entity)
	{
		T e = entity as T;
		if(e != null)
		{
			if(FilterRules.HasFilterPermission(e))
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