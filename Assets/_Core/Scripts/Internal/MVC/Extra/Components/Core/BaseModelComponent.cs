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
			return Components.Model;
		}
	}

	public ModelComponentState ComponentState
	{
		get; private set;
	}

	public T GetComponent<T>() where T : BaseModelComponent
	{
		return Components.GetComponent<T>();
	}

	public void Initialize(ModelComponents parent)
	{
		if(ComponentState != ModelComponentState.None)
			return;

		Components = parent;
		ComponentState = ModelComponentState.Initialized;
		Added();
	}

	public void SignalReady()
	{
		if(ComponentState != ModelComponentState.Initialized)
			return;

		ComponentState = ModelComponentState.Active;
		Ready();
	}

	public void Deinitialize()
	{
		if(ComponentState == ModelComponentState.Removed)
			return;

		ComponentState = ModelComponentState.Removed;
		Removed();
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
}