
public class TargetingWordItemModificationModel : BaseModel
{
	public delegate void TargetWordUIItemHandler(WordUIDisplayItemModel item, int index);
	public delegate void TargetingHandler(TargetSystem newTargetingSystem, TargetSystem previousTargetingSystem);
	public event TargetWordUIItemHandler CharAtItemIndexTypedEvent;
	public event TargetingHandler TargetingSystemChangedEvent;

	private WordsDisplayerModel _wordsDisplayerModel;
	private EntityFilter<EntityModel> _targetingEntityModels;

	private TargetSystem _activeTargetSystems;

	public TargetingWordItemModificationModel(WordsDisplayerModel wordsDisplayerModel)
	{
		FilterRules targetingEntityModelRules;
		FilterRules.OpenConstructHasAnyTags(Tags.DISPLAY_TARGETING);
		FilterRules.AddComponentToConstruct<TargetSystem>();
		FilterRules.CloseConstruct(out targetingEntityModelRules);

		_targetingEntityModels = EntityFilter<EntityModel>.Create(targetingEntityModelRules);
		_targetingEntityModels.TrackedEvent += OnTrackedEvent;
		_targetingEntityModels.UntrackedEvent += OnUntrackedEvent;

		_wordsDisplayerModel = wordsDisplayerModel;
	}

	public WordUIDisplayItemModel GetWordUIItemForEntityModel(EntityModel entityModel)
	{
		if(_wordsDisplayerModel != null)
		{
			return _wordsDisplayerModel.GetItemForEntityModel(entityModel);
		}

		return null;
	}

	protected override void OnModelDestroy()
	{
		_targetingEntityModels.TrackedEvent -= OnTrackedEvent;
		_targetingEntityModels.UntrackedEvent -= OnUntrackedEvent;
		_targetingEntityModels = null;

		_wordsDisplayerModel = null;
	}

	private void OnTrackedEvent(EntityModel entity)
	{
		if(_activeTargetSystems != null)
		{
			return;
		}

		TargetSystem targetSystem = entity.GetComponent<TargetSystem>();
		TargetSystem pre = _activeTargetSystems;
		_activeTargetSystems = targetSystem;
		_activeTargetSystems.TargetCharTypedEvent += OnTargetCharTypedEvent;

		if(TargetingSystemChangedEvent != null)
		{
			TargetingSystemChangedEvent(targetSystem, pre);
		}
	}

	private void OnUntrackedEvent(EntityModel entity)
	{
		TargetSystem targetSystem = entity.GetComponent<TargetSystem>();

		if(_activeTargetSystems == targetSystem)
		{
			TargetSystem pre = _activeTargetSystems;
			_activeTargetSystems.TargetCharTypedEvent -= OnTargetCharTypedEvent;
			_activeTargetSystems = null;

			if(TargetingSystemChangedEvent != null)
			{
				TargetingSystemChangedEvent(null, pre);
			}
		}
	}

	private void OnTargetCharTypedEvent(EntityModel target, char newChar, char requiredChar, int index)
	{
		WordUIDisplayItemModel item = GetWordUIItemForEntityModel(target);
		if(item != null && WordsHp.IsHit(newChar, requiredChar))
		{
			if(CharAtItemIndexTypedEvent != null)
			{
				CharAtItemIndexTypedEvent(item, index);
			}
		}
	}
}
