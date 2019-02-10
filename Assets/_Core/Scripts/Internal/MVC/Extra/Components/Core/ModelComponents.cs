using System;
using System.Collections.Generic;

public class ModelComponents : IComponentsHolder, IComponentsEnableModifier
{
	public enum ModelComponentsAction
	{
		AddComponent,
		RemoveComponent,
		EnableComponent,
		DisableComponent
	}

	public delegate bool ComponentActionHandler(ModelComponentsAction action, Type componentType, BaseModelComponent componentInstance);

	public event Action<BaseModelComponent> AddedComponentEvent;
	public event Action<BaseModelComponent> RemovedComponentEvent;
	public event Action<BaseModelComponent, bool> ChangedComponentEnabledStateEvent;

	public BaseModel Model
	{
		get; private set;
	}

	private HashSet<BaseModelComponent> _components = new HashSet<BaseModelComponent>();
	private List<BaseModelComponent> _removingComponents = new List<BaseModelComponent>();
	private List<BaseModelComponent> _disabledComponents = new List<BaseModelComponent>();

	private bool _isReady = false;
	private bool _isCleaning = false;
	private ComponentActionHandler _canPerformActionChecker;

	public ModelComponents(BaseModel model, ComponentActionHandler canPerformActionChecker)
	{
		Model = model;
		_canPerformActionChecker = canPerformActionChecker;
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
		if(_isCleaning)
			return;

		_isCleaning = true;
		foreach(BaseModelComponent component in _components)
		{
			InternalRemoveComponent(component, true);
		}

		for(int i = _removingComponents.Count - 1; i >= 0; i--)
		{
			_components.Remove(_removingComponents[i]);
			_disabledComponents.Remove(_removingComponents[i]);
		}
		
		_components.Clear();
		_removingComponents.Clear();

		AddedComponentEvent = null;
		RemovedComponentEvent = null;
		
		_components = null;
		_removingComponents = null;
		Model = null;
	}

	public bool TryIsEnabledCheck<T>(out bool isEnabled) where T : BaseModelComponent
	{
		return TryIsEnabledCheck(GetComponent<T>(), out isEnabled);
	}

	public bool TryIsEnabledCheck(BaseModelComponent component, out bool isEnabled)
	{
		if(component == null || !_components.Contains(component))
		{
			isEnabled = false;
			return false;
		}

		isEnabled = !_disabledComponents.Contains(component);
		return true;
	}

	public void SetComponentEnabledState<T>(bool enabledState) where T : BaseModelComponent
	{
		SetComponentEnabledState(GetComponent<T>(), enabledState);
	}

	public void SetComponentEnabledState(BaseModelComponent component, bool enabledState)
	{
		if(component == null || !_components.Contains(component))
			return;

		if(enabledState)
			if(!_canPerformActionChecker(ModelComponentsAction.EnableComponent, component.GetType(), component))
				return;
		else if(!_canPerformActionChecker(ModelComponentsAction.DisableComponent, component.GetType(), component))
				return;

		if(_disabledComponents.Contains(component) && !enabledState)
			return;

		if(component.IsEnabled == enabledState)
			return;

		if(enabledState)
			_disabledComponents.Remove(component);
		else
			_disabledComponents.Add(component);

		if(ChangedComponentEnabledStateEvent != null)
		{
			ChangedComponentEnabledStateEvent(component, enabledState);
		}
	}

	public T AddComponent<T>() where T : BaseModelComponent
	{
		return AddComponent(typeof(T)) as T;
	}

	public BaseModelComponent AddComponent(Type componentType)
	{
		if(!_canPerformActionChecker(ModelComponentsAction.AddComponent, componentType, null))
			return null;

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

	public bool RequireComponent<T>(out T component) where T : BaseModelComponent
	{
		BaseModelComponent comp;
		bool added = RequireComponent(typeof(T), out comp);
		component = comp as T;
		return added;
	}

	public bool RequireComponent(Type componentType, out BaseModelComponent component)
	{
		component = GetComponent(componentType);

		if(component == null)
		{
			component = AddComponent(componentType);
			return true;
		}

		return false;
	}

	public void RemoveComponent<T>() where T : BaseModelComponent
	{
		RemoveComponent(typeof(T));
	}

	public void RemoveComponent(Type componentType)
	{
		InternalRemoveComponent(GetComponent(componentType));
	}

	public void RemoveComponent(BaseModelComponent component)
	{
		InternalRemoveComponent(component);
	}

	public bool HasComponent<T>(bool incDisabledComponents = true) where T : BaseModelComponent
	{
		return HasComponent(typeof(T), incDisabledComponents);
	}

	public bool HasComponent(Type componentType, bool incDisabledComponents = true)
	{
		return InternalGetComponent(componentType, incDisabledComponents, false) != null;
	}

	public BaseModelComponent GetComponent(Type componentType)
	{
		return InternalGetComponent(componentType, true);
	}

	public T GetComponent<T>() where T : BaseModelComponent
	{
		return GetComponent(typeof(T)) as T;
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
		if(component != null && _canPerformActionChecker(ModelComponentsAction.RemoveComponent, component.GetType(), component))
		{
			if(!_removingComponents.Contains(component))
			{
				_removingComponents.Add(component);

				if(RemovedComponentEvent != null)
				{
					RemovedComponentEvent(component);
				}

				component.Deinitialize();
			}

			if(!selfClean && !_isCleaning)
			{
				for(int i = _removingComponents.Count - 1; i >= 0; i--)
				{
					_components.Remove(_removingComponents[i]);
					_disabledComponents.Remove(_removingComponents[i]);
				}
			}
		}
	}

	private BaseModelComponent InternalGetComponent(Type type, bool incDisabledComponents, bool incRemovingComponents = true)
	{
		foreach(BaseModelComponent component in _components)
		{
			if(!incDisabledComponents && _disabledComponents.Contains(component))
				continue;

			if(!incRemovingComponents && _removingComponents.Contains(component))
				continue;

			if(type.IsAssignableFrom(component.GetType()))
			{
				return component;
			}
		}

		return null;
	}
}

public interface IComponentsEnableModifier
{
	bool TryIsEnabledCheck<T>(out bool isEnabled) where T : BaseModelComponent;
	void SetComponentEnabledState<T>(bool enabledState) where T : BaseModelComponent;
}

public interface IComponentsHolder
{
	T AddComponent<T>() where T : BaseModelComponent;
	BaseModelComponent AddComponent(Type componentType);
	T GetComponent<T>() where T : BaseModelComponent;
	BaseModelComponent GetComponent(Type componentType);
	bool RequireComponent<T>(out T component) where T : BaseModelComponent;
	bool RequireComponent(Type componentType, out BaseModelComponent component);
	void RemoveComponent<T>() where T : BaseModelComponent;
	void RemoveComponent(Type componentType);
	void RemoveComponent(BaseModelComponent component);
	bool HasComponent<T>(bool incDisabledComponents = true) where T : BaseModelComponent;
	bool HasComponent(Type componentType, bool incDisabledComponents = true);
}