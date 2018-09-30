using System.Collections.Generic;
using System.Collections.ObjectModel;

public delegate void ModelTagHandler(BaseModel model, string tag);

public class ModelTags
{
    public event ModelTagHandler TagAddedEvent;
    public event ModelTagHandler TagRemovedEvent;

    public List<string> _tags = new List<string>();

    private BaseModel _parent;

    public ModelTags(BaseModel parent)
    {
        _parent = parent;
    }

    public void Clean()
    {
        for(int i = _tags.Count - 1; i >= 0; i--)
        {
            RemoveTag(_tags[i]);
        }

        _tags.Clear();
        _tags = null;
        _parent = null;

        TagAddedEvent = null;
        TagRemovedEvent = null;
    }

    public ReadOnlyCollection<string> GetTags()
    {
        return _tags.AsReadOnly();
    }

    public void AddTag(string tag)
    {
        if (!HasTag(tag) && !string.IsNullOrEmpty(tag))
        {
            _tags.Add(tag);

            if (TagAddedEvent != null)
            {
                TagAddedEvent(_parent, tag);
            }
        }
    }

    public void RemoveTag(string tag)
    {
        if (tag == null || string.IsNullOrEmpty(tag))
            return;

        if (HasTag(tag))
        {
            _tags.Remove(tag);

            if (TagRemovedEvent != null)
            {
                TagRemovedEvent(_parent, tag);
            }
        }
    }

    public bool HasTag(string tag)
    {
        if (tag == null || string.IsNullOrEmpty(tag))
            return _tags.Count == 0;

        return _tags.Contains(tag);
    }

    public bool HasAnyTag(params string[] tags)
    {
        if (tags == null || tags.Length == 0)
            return _tags.Count == 0;

        for (int i = 0, c = tags.Length; i < c; i++)
        {
            if (HasTag(tags[i]))
            {
                return true;
            }
        }

        return false;
    }

    public bool HasAllTags(params string[] tags)
    {
        if (tags == null || tags.Length == 0)
            return _tags.Count == 0;

        for (int i = 0, c = tags.Length; i < c; i++)
        {
            if (!HasTag(tags[i]))
            {
                return false;
            }
        }

        return true;
    }
}

public interface IModelTagsHolder
{
    ModelTags ModelTags { get; }
}
