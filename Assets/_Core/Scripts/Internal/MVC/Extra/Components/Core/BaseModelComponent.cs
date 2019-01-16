public enum ModelComponentState
{
	None,
	Initialized,
	Active,
	Removed
}

public abstract class BaseModelComponent
{
	public ModelComponents Components
	{
		get; private set;
	}

	public BaseModel Parent
	{
		get
		{
			if(Components == null)
				return null;

			return Components.Model;
		}
	}

	public bool IsEnabled
	{
		get
		{
			if(Components == null)
				return false;

			if(!_isEnabled.HasValue)
			{
				bool isEnabled;
				Components.TryIsEnabledCheck(this, out isEnabled);
				_isEnabled = isEnabled;
			}

			return _isEnabled.Value;
		}
		private set
		{
			_isEnabled = value;
		}
	}

	public ModelComponentState ComponentState
	{
		get; private set;
	}

	private bool? _isEnabled = null;

	public T GetComponent<T>() where T : BaseModelComponent
	{
		if(Components == null)
			return null;

		return Components.GetComponent<T>();
	}

	public void Initialize(ModelComponents parent)
	{
		if(ComponentState != ModelComponentState.None)
			return;

		Components = parent;
		Components.ChangedComponentEnabledStateEvent += OnChangedComponentActiveStateEvent;
		ComponentState = ModelComponentState.Initialized;
		Added();
	}

	public void SetActiveState(bool activeState)
	{
		if(Components != null)
		{
			Components.SetComponentEnabledState(this, activeState);
		}
	}

	public void SignalReady()
	{
		if(ComponentState != ModelComponentState.Initialized)
			return;

		ComponentState = ModelComponentState.Active;
		Ready();

		if(IsEnabled)
		{
			Enabled();
		}
		else
		{
			Disabled();
		}
	}

	public void Deinitialize()
	{
		if(ComponentState == ModelComponentState.Removed)
			return;

		bool wasActive = ComponentState == ModelComponentState.Active;

		ComponentState = ModelComponentState.Removed;
		Removed();

		if(IsEnabled && wasActive)
			Disabled();

		Components.ChangedComponentEnabledStateEvent -= OnChangedComponentActiveStateEvent;
		Components = null;
	}

	protected virtual void Added()
	{
	}
	protected virtual void Ready()
	{
	}
	protected virtual void Removed()
	{
	}
	protected virtual void Enabled()
	{
	}
	protected virtual void Disabled()
	{
	}

	private void OnChangedComponentActiveStateEvent(BaseModelComponent component, bool activeState)
	{
		if(component == this)
		{
			if(!_isEnabled.HasValue || _isEnabled.Value != activeState)
			{
				IsEnabled = activeState;
				if(IsEnabled)
				{
					Enabled();
				}
				else
				{
					Disabled();
				}
			}
		}
	}
}