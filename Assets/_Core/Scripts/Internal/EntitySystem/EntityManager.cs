using System.Collections.Generic;
using System.Collections.ObjectModel;
using System;

public class EntityManager
{
    public static EntityManager Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = new EntityManager();
            }

            return _instance;
        }
    }

    private static EntityManager _instance;

    private List<BaseModel> _entities = new List<BaseModel>();

    public Controller<M, V> LinkAndRegisterEntity<M, V>(M model, V view) where M : BaseModel where V : MonoBaseView
    {
        Controller<M, V> controller = Controller<M, V>.Link(model, view);
        model.DestroyEvent += OnDestroyEvent;
        _entities.Add(model);
        return controller;
    }


    private void OnDestroyEvent(BaseModel destroyedEntity)
    {
        _entities.Remove(destroyedEntity);
    }

    // -- Entity Query Methods -- \\

    // - Single Entity - \\

    public BaseModel GetAnEntity()
    {
        ReadOnlyCollection<BaseModel> e = GetEntities();
        if (e.Count > 0)
            return e[0];

        return null;
    }

    public BaseModel GetAnEntity(Func<BaseModel, bool> filterCondition)
    {
        ReadOnlyCollection<BaseModel> e = GetEntities(filterCondition);
        if (e.Count > 0)
            return e[0];

        return null;
    }

    public T GetAnEntity<T>() where T : BaseModel
    {
        ReadOnlyCollection<T> e = GetEntities<T>();
        if (e.Count > 0)
            return e[0];

        return null;
    }

    public T GetAnEntity<T>(Func<T, bool> filterCondition) where T : BaseModel
    {
        ReadOnlyCollection<T> e = GetEntities<T>(filterCondition);
        if (e.Count > 0)
            return e[0];

        return null;
    }

    // - Multiple Entities - \\

    public ReadOnlyCollection<BaseModel> GetEntities()
    {
        return _entities.AsReadOnly();
    }

    public ReadOnlyCollection<BaseModel> GetEntities(Func<BaseModel, bool> filterCondition)
    {
        return GetEntities<BaseModel>(filterCondition);
    }

    public ReadOnlyCollection<T> GetEntities<T>() where T : BaseModel
    {
        return GetEntities<T>(null);
    }

    public ReadOnlyCollection<T> GetEntities<T>(Func<T, bool> filterCondition) where T : BaseModel
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
