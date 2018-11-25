using System;
using System.Collections.Generic;

public enum TagFilterType
{
	None,
	HasAnyTag,
	HasAllTags,
}

public class FilterRules
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
	private static FilterRules _constructingFiltersParameters;

	public static void OpenFilterCreation()
	{
		if (_filterOpened)
		{
			throw new Exception("Tried opening a filter creation while a previous one has not yet been closed using `CloseFilterRulesCreation`");
		}
		else
		{
			_filterOpened = true;
		}
	}

	public static void CreateTagsFilterRules()
	{
		_constructingFiltersParameters = new FilterRules(new string[] { }, TagFilterType.None);
	}

	public static void CreateHasAnyTagsFilterRules(params string[] tags)
	{
		_constructingFiltersParameters = new FilterRules(tags, TagFilterType.HasAnyTag);
	}

	public static void CreateHasAllTagsFilterRules(params string[] tags)
	{
		_constructingFiltersParameters = new FilterRules(tags, TagFilterType.HasAllTags);
	}

	public static void AddComponentToFilterRules<T>() where T : BaseModelComponent
	{
		Type t = typeof(T);
		if (!_constructingFiltersParameters._componentsToFilterOn.Contains(t))
		{
			_constructingFiltersParameters._componentsToFilterOn.Add(t);
		}
	}

	public static void CloseFilterRulesCreation(out FilterRules filterCreated)
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

		if (!hasPermission)
		{
			return false;
		}

		for (int i = 0, c = _componentsToFilterOn.Count; i < c; i++)
		{
			if (!entity.HasComponent(_componentsToFilterOn[i]))
			{
				return false;
			}
		}

		return true;
	}

	public bool Equals(FilterRules filter)
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

	private FilterRules(string[] tags, TagFilterType tagFilterType)
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