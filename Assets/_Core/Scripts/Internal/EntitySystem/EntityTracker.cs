using System.Collections.Generic;
using System.Collections.ObjectModel;
using System;

public class EntityTracker
{
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

    private List<EntityModel> _entities = new List<EntityModel>();

    public void Register(EntityModel model)
    {
        if (_entities.Contains(model))
            return;

        model.DestroyEvent += OnDestroyEvent;
        _entities.Add(model);
    }

    public void Unregister(EntityModel model)
    {
        if (!_entities.Contains(model))
            return;

        model.DestroyEvent -= OnDestroyEvent;
        _entities.Remove(model);
    }


    private void OnDestroyEvent(BaseModel destroyedEntity)
    {
        _entities.Remove((EntityModel)destroyedEntity);
    }

    // -- Entity Query Methods -- \\

    // - Single Entity - \\

    public EntityModel GetAnEntity()
    {
        ReadOnlyCollection<EntityModel> e = GetEntities();
        if (e.Count > 0)
            return e[0];

        return null;
    }

    public EntityModel GetAnEntity(Func<EntityModel, bool> filterCondition)
    {
        ReadOnlyCollection<EntityModel> e = GetEntities(filterCondition);
        if (e.Count > 0)
            return e[0];

        return null;
    }

    public T GetAnEntity<T>() where T : EntityModel
    {
        ReadOnlyCollection<T> e = GetEntities<T>();
        if (e.Count > 0)
            return e[0];

        return null;
    }

    public T GetAnEntity<T>(Func<T, bool> filterCondition) where T : EntityModel
    {
        ReadOnlyCollection<T> e = GetEntities<T>(filterCondition);
        if (e.Count > 0)
            return e[0];

        return null;
    }

    // - Multiple Entities - \\

    public ReadOnlyCollection<EntityModel> GetEntities()
    {
        return _entities.AsReadOnly();
    }

    public ReadOnlyCollection<EntityModel> GetEntities(Func<EntityModel, bool> filterCondition)
    {
        return GetEntities<EntityModel>(filterCondition);
    }

    public ReadOnlyCollection<T> GetEntities<T>() where T : EntityModel
    {
        return GetEntities<T>(null);
    }

    public ReadOnlyCollection<T> GetEntities<T>(Func<T, bool> filterCondition) where T : EntityModel
    {
        List<T> result = new List<T>();
        for (int i = 0, count = _entities.Count; i < count; i++)
        {
            T e = _entities[i] as T;
            if (e != null && (filterCondition == null || filterCondition(e)))
            {
                result.Add(e);
            }
        }
        return result.AsReadOnly();
    }
}
