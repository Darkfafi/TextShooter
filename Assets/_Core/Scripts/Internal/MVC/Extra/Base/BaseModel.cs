using System;

/// <summary>
/// The design is to have the model contain no Unity specifications. It serves as a data object for its respective view.
/// </summary>
public abstract class BaseModel : IModel, IComponentsHolder, IComponentsEnableModifier
{
	public event Action<BaseModel> DestroyEvent;
	public event Action<BaseModel, BaseModelComponent> AddedComponentToModelEvent;
	public event Action<BaseModel, BaseModelComponent> RemovedComponentFromModelEvent;
	public event Action<BaseModel, BaseModelComponent, bool> ChangedComponentEnabledStateFromModelEvent;
	public event Action<BaseModel> ModelReadyEvent;

	public bool IsDestroyed
	{
		get; private set;
	}

	public IAbstractController LinkingController
	{
		get; private set;
	}

	public MethodPermitter MethodPermitter
	{
		get; private set;
	}
	
	private ModelComponents _components;
	private bool _internalDestroyCalled = false;

	public BaseModel()
	{
		MethodPermitter = new MethodPermitter();
		_components = new ModelComponents(this, ComponentActionValidation);
		_components.AddedComponentEvent += OnAddedComponentEvent;
		_components.RemovedComponentEvent += OnRemovedComponentEvent;
		_components.ChangedComponentEnabledStateEvent += OnChangedComponentEnabledStateEvent;

	}

	public void Destroy()
	{
		if(IsDestroyed)
		{
			return;
		}

		if(!_internalDestroyCalled)
		{
			_internalDestroyCalled = true;
			LinkingController.Destroy();
			return;
		}

		IsDestroyed = true;

		if(DestroyEvent != null)
		{
			DestroyEvent(this);
		}

		OnModelDestroy();

		_components.AddedComponentEvent -= OnAddedComponentEvent;
		_components.RemovedComponentEvent -= OnRemovedComponentEvent;
		_components.ChangedComponentEnabledStateEvent -= OnChangedComponentEnabledStateEvent;

		_components.Clean();
		_components = null;

		LinkingController = null;
	}

	public void SetupModel(IAbstractController controller)
	{
		if(LinkingController != null)
			return;

		LinkingController = controller;
		IsDestroyed = false;
		_components.SignalReady();
		OnModelReady();

		if(ModelReadyEvent != null)
		{
			ModelReadyEvent(this);
		}
	}

	protected virtual void OnModelReady()
	{
	}
	protected virtual void OnModelDestroy()
	{
	}

	protected virtual bool ComponentActionValidation(ModelComponents.ModelComponentsAction action, Type componentType)
	{
		return true;
	}

	public bool TryIsEnabledCheck<T>(out bool isEnabled) where T : BaseModelComponent
	{
		return _components.TryIsEnabledCheck<T>(out isEnabled);
	}

	public void SetComponentEnabledState<T>(bool enabledState) where T : BaseModelComponent
	{
		_components.SetComponentEnabledState<T>(enabledState);
	}

	public T AddComponent<T>() where T : BaseModelComponent
	{
		return _components.AddComponent<T>();
	}

	public BaseModelComponent AddComponent(Type componentType)
	{
		return _components.AddComponent(componentType);
	}

	public bool RequireComponent<T>(out T component) where T : BaseModelComponent
	{
		return _components.RequireComponent<T>(out component);
	}

	public bool RequireComponent(Type componentType, out BaseModelComponent component)
	{
		return _components.RequireComponent(componentType, out component);
	}

	public void RemoveComponent<T>() where T : BaseModelComponent
	{
		_components.RemoveComponent<T>();
	}

	public void RemoveComponent(Type componentType)
	{
		_components.RemoveComponent(componentType);
	}

	public void RemoveComponent(BaseModelComponent component)
	{
		_components.RemoveComponent(component);
	}

	public T GetComponent<T>() where T : BaseModelComponent
	{
		return _components.GetComponent<T>();
	}

	public BaseModelComponent GetComponent(Type componentType)
	{
		return _components.GetComponent(componentType);
	}

	public bool HasComponent<T>(bool incDisabledComponents = true) where T : BaseModelComponent
	{
		return _components.HasComponent<T>(incDisabledComponents);
	}

	public bool HasComponent(Type componentType, bool incDisabledComponents = true)
	{
		return _components.HasComponent(componentType, incDisabledComponents);
	}

	private void OnAddedComponentEvent(BaseModelComponent component)
	{
		if(AddedComponentToModelEvent != null)
		{
			AddedComponentToModelEvent(this, component);
		}
	}

	private void OnRemovedComponentEvent(BaseModelComponent component)
	{
		if(RemovedComponentFromModelEvent != null)
		{
			RemovedComponentFromModelEvent(this, component);
		}
	}

	private void OnChangedComponentEnabledStateEvent(BaseModelComponent component, bool enabledState)
	{
		if(ChangedComponentEnabledStateFromModelEvent != null)
		{
			ChangedComponentEnabledStateFromModelEvent(this, component, enabledState);
		}
	}
}
