using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public class ModelHolder<BM> where BM : BaseModel
{
    public event Action<BM> TrackedEvent;
    public event Action<BM> UntrackedEvent;

    private List<BM> _models = new List<BM>();

    // -- Entity Query Methods -- \\

    // - Single Entity - \\

    public BM GetAny()
    {
        ReadOnlyCollection<BM> e = GetAll();
        if (e.Count > 0)
            return e[0];

        return null;
    }

    public BM GetAny(Func<BM, bool> filterCondition)
    {
        ReadOnlyCollection<BM> e = GetAll(filterCondition);
        if (e.Count > 0)
            return e[0];

        return null;
    }

    public T GetAny<T>() where T : BM
    {
        ReadOnlyCollection<T> e = GetAll<T>();
        if (e.Count > 0)
            return e[0];

        return null;
    }

    public T GetAny<T>(Func<T, bool> filterCondition) where T : BM
    {
        ReadOnlyCollection<T> e = GetAll<T>(filterCondition);
        if (e.Count > 0)
            return e[0];

        return null;
    }

    // - Multiple Entities - \\

    public ReadOnlyCollection<BM> GetAll()
    {
        return _models.AsReadOnly();
    }

    public ReadOnlyCollection<BM> GetAll(Func<BM, bool> filterCondition)
    {
        return GetAll<BM>(filterCondition);
    }

    public ReadOnlyCollection<T> GetAll<T>() where T : BM
    {
        return GetAll<T>(null);
    }

    public ReadOnlyCollection<T> GetAll<T>(Func<T, bool> filterCondition) where T : BM
    {
        List<T> result = new List<T>();
        for (int i = 0, count = _models.Count; i < count; i++)
        {
            T e = _models[i] as T;
            if (e != null && (filterCondition == null || filterCondition(e)))
            {
                result.Add(e);
            }
        }
        return result.AsReadOnly();
    }

    public virtual void Clean()
    {
        for(int i = _models.Count - 1; i >= 0; i--)
        {
            Untrack(_models[i]);
        }

        _models.Clear();
        _models = null;
    }

    // Internal tracking

    protected bool Track(BM model)
    {
        if (_models.Contains(model))
            return false;

        _models.Add(model);

        if (TrackedEvent != null)
        {
            TrackedEvent(model);
        }

        return true;
    }

    protected bool Untrack(BM model)
    {
        if (!_models.Contains(model))
            return false;

        _models.Remove(model);

        if (UntrackedEvent != null)
        {
            UntrackedEvent(model);
        }

        return true;
    }
}