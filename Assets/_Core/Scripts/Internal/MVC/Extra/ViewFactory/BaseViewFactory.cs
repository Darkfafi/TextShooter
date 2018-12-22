using System;

public abstract class BaseViewFactory<M, V> where M : EntityModel where V : class, IView
{
	public event Action<M, V> CreatedViewForModelEvent;

	private EntityFilter<M> _factoryFilter;
	private bool _isAutomatic = false;

	public BaseViewFactory(bool automaticState)
	{
		_factoryFilter = EntityFilter<M>.Create();
		SetAutomaticState(automaticState);
	}

	public BaseViewFactory(bool automaticState, FilterRules filterRules)
	{
		_factoryFilter = EntityFilter<M>.Create(filterRules);
		SetAutomaticState(automaticState);
	}

	~BaseViewFactory()
	{
		if(_factoryFilter != null)
		{
			SetAutomaticState(false);
			_factoryFilter.Clean();
			_factoryFilter = null;
		}
	}

	public void SetAutomaticState(bool automaticState)
	{
		if(_isAutomatic == automaticState)
			return;

		_isAutomatic = automaticState;

		if(_isAutomatic)
			_factoryFilter.TrackedEvent += OnTrackedEvent;
		else
			_factoryFilter.TrackedEvent -= OnTrackedEvent;
	}

	private void OnTrackedEvent(M trackedModel)
	{
		CreateViewForModel(trackedModel);
	}

	public V CreateViewForModel(M model)
	{
		V view = ConstructViewForModel(model);
		Controller.Link(model, view);

		if(CreatedViewForModelEvent != null)
		{
			CreatedViewForModelEvent(model, view);
		}

		return view;
	}

	protected abstract V ConstructViewForModel(M model);
}
