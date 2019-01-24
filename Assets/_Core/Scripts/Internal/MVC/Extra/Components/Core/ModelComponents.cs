﻿using System;
using System.Collections.Generic;

public class ModelComponents : IComponentsHolder
{
	public enum ModelComponentsAction
	{
		AddComponent,
		RemoveComponent,
		EnableComponent,
		DisableComponent
	}

	public delegate bool ComponentActionHandler(ModelComponentsAction action, Type componentType);

	public event Action<BaseModelComponent> AddedComponentEvent;
	public event Action<BaseModelComponent> RemovedComponentEvent;
	public event Action<BaseModelComponent, bool> ChangedComponentEnabledStateEvent;

	public BaseModel Model
	{
		get; private set;
	}

	private HashSet<BaseModelComponent> _enabledComponents = new HashSet<BaseModelComponent>();
	private HashSet<BaseModelComponent> _disabledComponents = new HashSet<BaseModelComponent>();
	private List<BaseModelComponent> _removingComponents = new List<BaseModelComponent>();
	private bool _isReady = false;
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
		foreach(BaseModelComponent component in _enabledComponents)
		{
			component.SignalReady();
		}

		foreach(BaseModelComponent component in _disabledComponents)
		{
			component.SignalReady();
		}
	}

	public void Clean()
	{
		foreach(BaseModelComponent component in _enabledComponents)
		{
			InternalRemoveComponent(component, true);
		}

		foreach(BaseModelComponent component in _disabledComponents)
		{
			InternalRemoveComponent(component, true);
		}

		for(int i = _removingComponents.Count - 1; i >= 0; i--)
		{
			_enabledComponents.Remove(_removingComponents[i]);
			_disabledComponents.Remove(_removingComponents[i]);
		}

		_disabledComponents.Clear();
		_enabledComponents.Clear();
		_removingComponents.Clear();

		AddedComponentEvent = null;
		RemovedComponentEvent = null;

		_disabledComponents = null;
		_enabledComponents = null;
		_removingComponents = null;
		Model = null;
	}

	public bool TryIsEnabledCheck<T>(out bool isEnabled) where T : BaseModelComponent
	{
		return TryIsEnabledCheck(GetComponent<T>(), out isEnabled);
	}

	public bool TryIsEnabledCheck(BaseModelComponent component, out bool isEnabled)
	{
		if(component == null)
		{
			isEnabled = false;
			return false;
		}

		if(_enabledComponents.Contains(component))
		{
			isEnabled = true;
		}
		else if(_disabledComponents.Contains(component))
		{
			isEnabled = false;
		}
		else
		{
			isEnabled = false;
			return false;
		}

		return true;
	}

	public void SetComponentEnabledState<T>(bool enabledState) where T : BaseModelComponent
	{
		SetComponentEnabledState(GetComponent<T>(), enabledState);
	}

	public void SetComponentEnabledState(BaseModelComponent component, bool enabledState)
	{
		if(component == null)
			return;

		if(enabledState)
			if(!_canPerformActionChecker(ModelComponentsAction.EnableComponent, component.GetType()))
				return;
		else if(!_canPerformActionChecker(ModelComponentsAction.DisableComponent, component.GetType()))
				return;

		bool hasComponent = false;
		if(_enabledComponents.Contains(component))
		{
			hasComponent = true;
			if(enabledState)
				return;
		}

		if(_disabledComponents.Contains(component))
		{
			hasComponent = true;
			if(!enabledState)
				return;
		}

		if(!hasComponent || component.IsEnabled == enabledState)
			return;

		if(enabledState)
		{
			_disabledComponents.Remove(component);
			_enabledComponents.Add(component);
		}
		else
		{
			_enabledComponents.Remove(component);
			_disabledComponents.Add(component);
		}

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
		if(!_canPerformActionChecker(ModelComponentsAction.AddComponent, componentType))
			return null;

		BaseModelComponent c = Activator.CreateInstance(componentType) as BaseModelComponent;

		if(c != null)
		{
			_enabledComponents.Add(c);
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
		if(component != null && _canPerformActionChecker(ModelComponentsAction.RemoveComponent, component.GetType()))
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

			if(!selfClean)
			{
				for(int i = _removingComponents.Count - 1; i >= 0; i--)
				{
					_enabledComponents.Remove(_removingComponents[i]);
					_disabledComponents.Remove(_removingComponents[i]);
				}
			}
		}
	}

	private BaseModelComponent InternalGetComponent(Type type, bool incDisabledComponents, bool incRemovingComponents = true)
	{
		if(incDisabledComponents)
		{
			foreach(BaseModelComponent component in _disabledComponents)
			{
				if(!incRemovingComponents && _removingComponents.Contains(component))
					continue;
				if(type.IsAssignableFrom(component.GetType()))
				{
					return component;
				}
			}
		}

		foreach(BaseModelComponent component in _enabledComponents)
		{
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

public interface IComponentsHolder
{
	T AddComponent<T>() where T : BaseModelComponent;
	BaseModelComponent AddComponent(Type componentType);
	void RemoveComponent<T>() where T : BaseModelComponent;
	void RemoveComponent(Type componentType);
	void RemoveComponent(BaseModelComponent component);
	T GetComponent<T>() where T : BaseModelComponent;
	BaseModelComponent GetComponent(Type componentType);
	bool TryIsEnabledCheck<T>(out bool isEnabled) where T : BaseModelComponent;
	void SetComponentEnabledState<T>(bool enabledState) where T : BaseModelComponent;
	bool HasComponent<T>(bool incDisabledComponents = true) where T : BaseModelComponent;
	bool HasComponent(Type componentType, bool incDisabledComponents = true);
}