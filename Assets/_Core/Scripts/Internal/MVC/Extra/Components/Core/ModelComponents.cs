using System;
using System.Collections.Generic;

public class ModelComponents : IComponentsHolder
{
    public BaseModel Model { get; private set; }
    private HashSet<BaseModelComponent> _components = new HashSet<BaseModelComponent>();
    private List<BaseModelComponent> _removingComponents = new List<BaseModelComponent>();
    private bool _isReady = false;

    public ModelComponents(BaseModel model)
    {
        Model = model;
    }

    public void SignalReady()
    {
        if (_isReady)
            return;

        _isReady = true;

        // Ready all already added components
        foreach (BaseModelComponent component in _components)
        {
            component.SignalReady();
        }
    }

    public void Clean()
    {
        foreach (BaseModelComponent component in _components)
        {
            InternalRemoveComponent(component, true);
        }

        for (int i = _removingComponents.Count - 1; i >= 0; i--)
        {
            _components.Remove(_removingComponents[i]);
        }

        _components.Clear();
        _removingComponents.Clear();

        _components = null;
        _removingComponents = null;
        Model = null;
    }

    public T AddComponent<T>() where T : BaseModelComponent
    {
        BaseModelComponent c = Activator.CreateInstance<T>();
        _components.Add(c);
        c.Initialize(this);

        if (_isReady)
        {
            c.SignalReady();
        }

        return c as T;
    }

    public void RemoveComponent<T>() where T : BaseModelComponent
    {
        BaseModelComponent c = GetComponentOfType(typeof(T));
        InternalRemoveComponent(c);
    }

    public T GetComponent<T>() where T : BaseModelComponent
    {
        return GetComponentOfType(typeof(T)) as T;
    }

    private BaseModelComponent GetComponentOfType(Type type)
    {
        foreach (BaseModelComponent component in _components)
        {
            if(type.IsAssignableFrom(component.GetType()))
            {
                return component;
            }
        }

        return null;
    }

    private void InternalRemoveComponent(BaseModelComponent component, bool selfClean = false)
    {
        if (component != null && !_removingComponents.Contains(component))
        {
            _removingComponents.Add(component);
            component.Deinitialize();
        }

        if (!selfClean)
        {
            for (int i = _removingComponents.Count - 1; i >= 0; i--)
            {
                _components.Remove(_removingComponents[i]);
            }
        }
    }
}


public interface IComponentsHolder
{
    T AddComponent<T>() where T : BaseModelComponent;
    void RemoveComponent<T>() where T : BaseModelComponent;
    T GetComponent<T>() where T : BaseModelComponent;
}