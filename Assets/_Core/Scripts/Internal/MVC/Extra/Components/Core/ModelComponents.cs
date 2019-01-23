using System;
using System.Collections.Generic;

public class ModelComponents : IComponentsHolder
{
	public event Action<BaseModelComponent> AddedComponentEvent;
	public event Action<BaseModelComponent> RemovedComponentEvent;

	public BaseModel Model
	{
		get; private set;
	}

	private HashSet<BaseModelComponent> _components = new HashSet<BaseModelComponent>();
	private List<BaseModelComponent> _removingComponents = new List<BaseModelComponent>();
	private bool _isReady = false;

	public ModelComponents(BaseModel model)
	{
		Model = model;
	}

	public void SignalReady()
	{
		if(_isReady)
			return;

		_isReady = true;

		// Ready all already added components
		foreach(BaseModelComponent component in _components)
		{
			component.SignalReady();
		}
	}

	public void Clean()
	{
		foreach(BaseModelComponent component in _components)
		{
			InternalRemoveComponent(component, true);
		}

		for(int i = _removingComponents.Count - 1; i >= 0; i--)
		{
			_components.Remove(_removingComponents[i]);
		}

		_components.Clear();
		_removingComponents.Clear();

		AddedComponentEvent = null;
		RemovedComponentEvent = null;

		_components = null;
		_removingComponents = null;
		Model = null;
	}

	public T AddComponent<T>() where T : BaseModelComponent
	{
		return AddComponent(typeof(T)) as T;
	}

	public BaseModelComponent AddComponent(Type componentType)
	{
		BaseModelComponent c = Activator.CreateInstance(componentType) as BaseModelComponent;
		if(c != null)
		{

			_components.Add(c);
			c.Initialize(this);

			if(AddedComponentEvent != null)
			{
				AddedComponentEvent(c);
			}

			if(_isReady)
			{
				c.SignalReady();
			}

			return c;
		}

		return null;
	}

	public void RemoveComponent<T>() where T : BaseModelComponent
	{
		RemoveComponent(typeof(T));
	}

	public void RemoveComponent(Type componentType)
	{
		RemoveComponent(GetComponent(componentType));
	}

	public void RemoveComponent(BaseModelComponent component)
	{
		InternalRemoveComponent(component);
	}

	public T GetComponent<T>() where T : BaseModelComponent
	{
		return GetComponent(typeof(T)) as T;
	}

	public BaseModelComponent GetComponent(Type type)
	{
		foreach(BaseModelComponent component in _components)
		{
			if(type.IsAssignableFrom(component.GetType()))
			{
				return component;
			}
		}

		return null;
	}

	public bool HasComponent<T>() where T : BaseModelComponent
	{
		return GetComponent<T>() != null;
	}

	public bool HasComponent(Type componentType)
	{
		return GetComponent(componentType) != null;
	}

	private void InternalRemoveComponent(BaseModelComponent component, bool selfClean = false)
	{
		if(component != null && !_removingComponents.Contains(component))
		{
			_removingComponents.Add(component);

			if(RemovedComponentEvent != null)
			{
				RemovedComponentEvent(component);
			}

			component.Deinitialize();
		}

		if(!selfClean)
		{
			for(int i = _removingComponents.Count - 1; i >= 0; i--)
			{
				_components.Remove(_removingComponents[i]);
			}
		}
	}
}


public interface IComponentsHolder
{
	T AddComponent<T>() where T : BaseModelComponent;
	BaseModelComponent AddComponent(Type componentType);
	void RemoveComponent<T>() where T : BaseModelComponent;
	void RemoveComponent(Type componentType);
	void RemoveComponent(BaseModelComponent component);
	T GetComponent<T>() where T : BaseModelComponent;
	BaseModelComponent GetComponent(Type componentType);
	bool HasComponent<T>() where T : BaseModelComponent;
	bool HasComponent(Type componentType);
}