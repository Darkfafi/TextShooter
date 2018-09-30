using System.Collections.Generic;

public class EntityModel : BaseModel, IModelTransformHolder
{
    public delegate void EntityTagHandler(EntityModel entity, string tag);
    public event EntityTagHandler TagAddedEvent;
    public event EntityTagHandler TagRemovedEvent;

    public List<string> _tags = new List<string>();

    public EntityModel()
    {
        ModelTransform = new ModelTransform();
        EntityTracker.Instance.Register(this);
    }

    public ModelTransform ModelTransform
    {
        get; private set;
    }

    public void AddTag(string tag)
    {
        if(!HasTag(tag) && !string.IsNullOrEmpty(tag))
        {
            _tags.Add(tag);

            if(TagAddedEvent != null)
            {
                TagAddedEvent(this, tag);
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

            if(TagRemovedEvent != null)
            {
                TagRemovedEvent(this, tag);
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

        for(int i = 0, c = tags.Length; i < c; i++)
        {
            if(HasTag(tags[i]))
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