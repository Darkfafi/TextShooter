using System;
using System.Collections.Generic;

public enum TagFilterType
{
	HasAnyTag,
	HasAllTags,
	HasNoneOfTag,
}

public struct FilterRules
{
	private List<TagRule> _filterTags;
	private List<IncComponentRule> _componentsToFilterOn;

	private static bool _filterOpened = false;
	private static FilterRules _constructingFiltersParameters;

	/// <summary>
	/// Creates a FilterRules with no tags to filter on
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
	/// Sets up Construct on given filterRules
	/// </summary>
	public static void OpenConstructOnFilterRules(FilterRules filterRules)
	{
		if(OpenFilterConstruct())
		{
			_constructingFiltersParameters = filterRules;
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
	/// Creates a FilterRules which will filter getting elements which have NONE of the given tags (TagFilterType.HasNoneOfTags)
	/// </summary>
	public static void OpenConstructNoneOfTags(string tag, params string[] tags)
	{
		if(OpenFilterConstruct())
		{
			_constructingFiltersParameters = CreateHasNoneOfTagsFilter(tag, tags);
		}
	}

	/// <summary>
	/// Adds a component type to the filter, so it will only get entries with the given component present.
	/// The filter will return entries which have ALL of the components given to it.
	/// </summary>
	public static void AddComponentToConstruct<T>(bool mustBeEnabled) where T : BaseModelComponent
	{
		Type t = typeof(T);
		IncComponentRule rule = new IncComponentRule(t, mustBeEnabled);
		if(!_constructingFiltersParameters._componentsToFilterOn.Contains(rule))
		{
			_constructingFiltersParameters._componentsToFilterOn.Add(rule);
		}
	}

	/// <summary>
	/// Adds a tag to the filter, so it will filter with the given tag associated with the given tag filter type 
	/// The filter will return entries which are valid to all Tag rules given to it.
	/// </summary>
	public static void AddTagToConstruct(string tag, TagFilterType filterType)
	{
		TagRule rule = new TagRule(tag, filterType);
		if(!_constructingFiltersParameters._filterTags.Contains(rule) && rule.Valid)
		{
			_constructingFiltersParameters._filterTags.Add(rule);
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
		_constructingFiltersParameters = default(FilterRules);
	}

	/// <summary>
	/// Creates a FilterRules with no tags to filter on (TagFilterType.None)
	/// This FilterRules is not open to any static construction methods. 
	/// </summary>
	/// <returns>Default FilterRules of (TagFilterType.None)</returns>
	public static FilterRules CreateNoTagsFilter()
	{
		return new FilterRules(new string[] { }, TagFilterType.HasAnyTag);
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
	/// Creates a FilterRules which will filter getting elements which have None of the given tags (TagFilterType.HasNoneOfTags)
	/// This FilterRules is not open to any static construction methods. 
	/// </summary>
	/// <returns>Default FilterRules of (TagFilterType.HasNoneOfTags)</returns>
	public static FilterRules CreateHasNoneOfTagsFilter(string tag, params string[] tags)
	{
		List<string> myTags = new List<string>(tags);
		myTags.Add(tag);
		return new FilterRules(myTags.ToArray(), TagFilterType.HasNoneOfTag);
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
		bool hasPermission = true;

		List<string> anyTagsToCheck = new List<string>();
		List<string> allTagsToCheck = new List<string>();
		List<string> noneTagsToCheck = new List<string>();

		for(int i = 0, c = _filterTags.Count; i < c; i++)
		{
			switch(_filterTags[i].TagFilterType)
			{
				case TagFilterType.HasAnyTag:
					anyTagsToCheck.Add(_filterTags[i].Tag);
					break;
				case TagFilterType.HasAllTags:
					allTagsToCheck.Add(_filterTags[i].Tag);
					break;
				case TagFilterType.HasNoneOfTag:
					noneTagsToCheck.Add(_filterTags[i].Tag);
					break;
			}
		}

		if(anyTagsToCheck.Count > 0)
		{
			hasPermission = entity.ModelTags.HasAnyTag(anyTagsToCheck.ToArray());
		}

		if(allTagsToCheck.Count > 0)
		{
			hasPermission = entity.ModelTags.HasAllTags(allTagsToCheck.ToArray());
		}

		if(noneTagsToCheck.Count > 0)
		{
			hasPermission = !entity.ModelTags.HasAnyTag(noneTagsToCheck.ToArray());
		}

		if(!hasPermission)
		{
			return false;
		}

		for(int i = 0, c = _componentsToFilterOn.Count; i < c; i++)
		{
			if(!entity.HasComponent(_componentsToFilterOn[i].ComponentType, !_componentsToFilterOn[i].MustBeEnabled))
			{
				return false;
			}
		}

		return true;
	}

	public bool Equals(FilterRules filter)
	{
		if(_filterTags.Count == filter._filterTags.Count && _componentsToFilterOn.Count == filter._componentsToFilterOn.Count)
		{
			for(int i = 0, c = _filterTags.Count; i < c; i++)
			{
				TagRule ownRule = _filterTags[i];
				if(filter._filterTags.FindIndex(fc => ownRule.IsEqual(fc)) < 0)
				{
					return false;
				}
			}

			for(int i = 0, c = _componentsToFilterOn.Count; i < c; i++)
			{
				IncComponentRule ownRule = _componentsToFilterOn[i];
				if(filter._componentsToFilterOn.FindIndex(fc => ownRule.IsEqual(fc)) < 0)
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
		_filterTags = new List<TagRule>();
		_componentsToFilterOn = new List<IncComponentRule>();

		for(int i = 0; i < tags.Length; i++)
		{
			_filterTags.Add(new TagRule(tags[i], tagFilterType));
		}
	}

	private struct IncComponentRule
	{
		public Type ComponentType
		{
			get; private set;
		}

		public bool MustBeEnabled
		{
			get; private set;
		}

		public bool Valid
		{
			get
			{
				return ComponentType != null;
			}
		}

		public IncComponentRule(Type componentType, bool mustBeEnabled)
		{
			ComponentType = componentType;
			MustBeEnabled = mustBeEnabled;
		}

		public bool IsEqual(IncComponentRule otherRule)
		{
			return ComponentType == otherRule.ComponentType && MustBeEnabled == otherRule.MustBeEnabled;
		}
	}

	private struct TagRule
	{
		public string Tag
		{
			get; private set;
		}

		public TagFilterType TagFilterType
		{
			get; private set;
		}

		public bool Valid
		{
			get
			{
				return !string.IsNullOrEmpty(Tag);
			}
		}

		public TagRule(string tag, TagFilterType filterType)
		{
			Tag = tag;
			TagFilterType = filterType;
		}

		public bool IsEqual(TagRule otherRule)
		{
			return Tag == otherRule.Tag && TagFilterType == otherRule.TagFilterType;
		}
	}
}