using System;
using UnityEngine;

public abstract class BaseMonoViewFactory<M, V> : MonoBehaviour where M : EntityModel where V : EntityView
{
	[SerializeField]
	private bool _initializeAsAutomatic = true;

	private InnerMonoViewFactory _factory;

	protected void Awake()
	{
		_factory = new InnerMonoViewFactory(GetViewPrefabForModel, _initializeAsAutomatic, CreateFilterRulesForFactory());
		_factory.CreatedViewForModelEvent += OnCreatedViewForModelEvent;
	}

	protected void OnDestroy()
	{
		_factory.SetAutomaticState(false);
		_factory.CreatedViewForModelEvent -= OnCreatedViewForModelEvent;
		_factory = null;
	}

	protected abstract V GetViewPrefabForModel(M model);

	protected virtual void OnViewConstructedForModel(M model, V view)
	{

	}

	protected virtual FilterRules CreateFilterRulesForFactory()
	{
		return FilterRules.CreateNoTagsFilter();
	}

	private void OnCreatedViewForModelEvent(M model, V view)
	{
		OnViewConstructedForModel(model, view);
	}

	private class InnerMonoViewFactory : BaseViewFactory<M, V>
	{
		private Func<M ,V> _prefabGetter;

		public InnerMonoViewFactory(Func<M, V> prefabGetter, bool automaticState, FilterRules filterRules) : base(automaticState, filterRules)
		{
			_prefabGetter = prefabGetter;
		}

		~InnerMonoViewFactory()
		{
			_prefabGetter = null;
		}

		protected override V ConstructViewForModel(M model)
		{
			if(_prefabGetter != null)
				return Instantiate(_prefabGetter(model));

			return null;
		}
	}
}