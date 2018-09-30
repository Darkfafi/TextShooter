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

    public BM GetAny(Comparison<BM> sort = null)
    {
        ReadOnlyCollection<BM> e = GetAll(sort);
        if (e.Count > 0)
            return e[0];

        return null;
    }

    public BM GetAny(Func<BM, bool> filterCondition, Comparison<BM> sort = null)
    {
        ReadOnlyCollection<BM> e = GetAll(filterCondition, sort);
        if (e.Count > 0)
            return e[0];

        return null;
    }

    public T GetAny<T>(Comparison<T> sort = null) where T : BM
    {
        ReadOnlyCollection<T> e = GetAll(sort);
        if (e.Count > 0)
            return e[0];

        return null;
    }

    public T GetAny<T>(Func<T, bool> filterCondition, Comparison<T> sort = null) where T : BM
    {
        ReadOnlyCollection<T> e = GetAll<T>(filterCondition, sort);
        if (e.Count > 0)
            return e[0];

        return null;
    }

    // - Multiple Entities - \\

    public ReadOnlyCollection<BM> GetAll(Comparison<BM> sort = null)
    {
        if (sort == null)
            return _models.AsReadOnly();
        else
            return GetAll(null, sort);
    }

    public ReadOnlyCollection<BM> GetAll(Func<BM, bool> filterCondition, Comparison<BM> sort = null)
    {
        return GetAll<BM>(filterCondition, sort);
    }

    public ReadOnlyCollection<T> GetAll<T>(Comparison<T> sort = null) where T : BM
    {
        return GetAll(null, sort);
    }

    public ReadOnlyCollection<T> GetAll<T>(Func<T, bool> filterCondition, Comparison<T> sort = null) where T : BM
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

        if(sort != null)
            result.Sort(sort);

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