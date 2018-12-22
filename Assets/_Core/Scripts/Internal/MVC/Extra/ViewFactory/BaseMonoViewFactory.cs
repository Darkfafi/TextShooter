using UnityEngine;

public abstract class BaseMonoViewFactory<M, V> : MonoBehaviour where M : EntityModel where V : EntityView
{
	[SerializeField]
	private V _entityViewPrefab;

	[SerializeField]
	private bool _initializeAsAutomatic = true;

	private InnerMonoViewFactory _factory;

	protected void Awake()
	{
		_factory = new InnerMonoViewFactory(_entityViewPrefab, _initializeAsAutomatic);
		_factory.CreatedViewForModelEvent += OnCreatedViewForModelEvent;
	}

	protected void OnDestroy()
	{
		_factory.SetAutomaticState(false);
		_factory.CreatedViewForModelEvent -= OnCreatedViewForModelEvent;
		_factory = null;
	}

	protected abstract void OnViewConstructedForModel(M model, V view);

	private void OnCreatedViewForModelEvent(M model, V view)
	{
		OnViewConstructedForModel(model, view);
	}

	private class InnerMonoViewFactory : BaseViewFactory<M, V>
	{
		private V _prefab;

		public InnerMonoViewFactory(V prefab, bool automaticState) : base(automaticState)
		{
			_prefab = prefab;
		}

		~InnerMonoViewFactory()
		{
			_prefab = null;
		}

		protected override V ConstructViewForModel(M model)
		{
			if(_prefab != null)
				return Instantiate(_prefab);

			return null;
		}
	}
}
