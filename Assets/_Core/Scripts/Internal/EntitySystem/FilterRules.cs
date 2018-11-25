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
	public TagFilterType FilterType
	{
		get; private set;
	}

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

	/// <summary>
	/// Creates a FilterRules with no tags to filter on (TagFilterType.None)
	/// </summary>
	public static void OpenConstructNoTags()
	{
		if(OpenFilterConstruct())
		{
			_constructingFiltersParameters = CreateNoTagsFilter();
		}
	}

	/// <summary>
	/// Creates a FilterRules which will filter getting elements which have ANY of the given tags (TagFilterType.HasAnyTag)
	/// </summary>
	public static void OpenConstructHasAnyTags(string tag, params string[] tags)
	{
		if(OpenFilterConstruct())
		{
			_constructingFiltersParameters = CreateHasAnyTagsFilter(tag, tags);
		}
	}

	/// <summary>
	/// Creates a FilterRules which will filter getting elements which have ALL of the given tags (TagFilterType.HasAllTags)
	/// </summary>
	public static void OpenConstructHasAllTags(string tag, params string[] tags)
	{
		if(OpenFilterConstruct())
		{
			_constructingFiltersParameters = CreateHasAllTagsFilter(tag, tags);
		}
	}

	/// <summary>
	/// Adds a component type to the filter, so it will only get entries with the given component present.
	/// The filter will return entries which have ALL of the components given to it.
	/// </summary>
	public static void AddComponentToConstruct<T>() where T : BaseModelComponent
	{
		Type t = typeof(T);
		if(!_constructingFiltersParameters._componentsToFilterOn.Contains(t))
		{
			_constructingFiltersParameters._componentsToFilterOn.Add(t);
		}
	}

	/// <summary>
	/// Closes the creation of the filter and gives the constructed filter, using the static methods, into the out parameter
	/// </summary>
	/// <param name="filterCreated"> The constructed Filter </param>
	public static void CloseConstruct(out FilterRules filterCreated)
	{
		filterCreated = _constructingFiltersParameters;
		_filterOpened = false;
		_constructingFiltersParameters = null;
	}

	/// <summary>
	/// Creates a FilterRules with no tags to filter on (TagFilterType.None)
	/// This FilterRules is not open to any static construction methods. 
	/// </summary>
	/// <returns>Default FilterRules of (TagFilterType.None)</returns>
	public static FilterRules CreateNoTagsFilter()
	{
		return new FilterRules(new string[] { }, TagFilterType.None);
	}

	/// <summary>
	/// Creates a FilterRules which will filter getting elements which have ANY of the given tags (TagFilterType.HasAnyTag)
	/// This FilterRules is not open to any static construction methods. 
	/// </summary>
	/// <returns>Default FilterRules of (TagFilterType.HasAnyTag)</returns>
	public static FilterRules CreateHasAnyTagsFilter(string tag, params string[] tags)
	{
		List<string> myTags = new List<string>(tags);
		myTags.Add(tag);
		return new FilterRules(myTags.ToArray(), TagFilterType.HasAnyTag);
	}

	/// <summary>
	/// Creates a FilterRules which will filter getting elements which have ALL of the given tags (TagFilterType.HasAllTags)
	/// This FilterRules is not open to any static construction methods. 
	/// </summary>
	/// <returns>Default FilterRules of (TagFilterType.HasAllTags)</returns>
	public static FilterRules CreateHasAllTagsFilter(string tag, params string[] tags)
	{
		List<string> myTags = new List<string>(tags);
		myTags.Add(tag);
		return new FilterRules(myTags.ToArray(), TagFilterType.HasAllTags);
	}

	/// <summary>
	/// This method allows you to create the FilterRules using the given static methods. To get the constructed FilterRules, call the `CloseFilterRulesCreation` method.
	/// </summary>
	private static bool OpenFilterConstruct()
	{
		if(_filterOpened)
		{
			throw new Exception("Tried opening a filter creation while a previous one has not yet been closed using `CloseFilterRulesCreation`");
		}
		else
		{
			_filterOpened = true;
			return true;
		}
	}

	public bool HasFilterPermission(EntityModel entity)
	{
		bool hasPermission = false;

		switch(FilterType)
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

	public bool Equals(FilterRules filter)
	{
		if(FilterType == filter.FilterType && FilterTags.Length == filter.FilterTags.Length && FilterComponents.Length == filter.FilterComponents.Length)
		{
			for(int i = 0, c = filter.FilterTags.Length; i < c; i++)
			{
				if(!_filterTags.Contains(filter.FilterTags[i]))
				{
					return false;
				}
			}

			for(int i = 0, c = filter.FilterComponents.Length; i < c; i++)
			{
				if(!_componentsToFilterOn.Contains(filter.FilterComponents[i]))
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
		_filterTags = new List<string>(tags);
		FilterType = tagFilterType;
	}
}