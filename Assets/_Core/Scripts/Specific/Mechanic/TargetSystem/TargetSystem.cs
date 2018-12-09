using System.Collections.Generic;

public class TargetSystem : BaseModelComponent
{
	public delegate void NewOldTargetHandler(EntityModel newTarget, EntityModel previousTarget);
	public delegate void TargetHandler(EntityModel target);
	public event NewOldTargetHandler TargetSetEvent;
	public event TargetHandler TargetCompletedEvent;

	private EntityModel _currentTypingTarget;
	private string _buildupShootString = "";

	private List<EntityModel> _completedTargetList = new List<EntityModel>();

	private CharInputModel _charInputModel;
	private EntityFilter<EntityModel> _targetFilter;

	public void SetupTargetSystem(CharInputModel charInputModel, FilterRules filterRules)
	{
		if(_targetFilter != null)
			return;

		FilterRules.OpenConstructOnFilterRules(filterRules);
		FilterRules.AddComponentToConstruct<WordsHolder>();
		FilterRules.AddComponentToConstruct<WordsHp>();
		FilterRules.CloseConstruct(out filterRules);

		_targetFilter = EntityFilter<EntityModel>.Create(filterRules);
		_targetFilter.UntrackedEvent += OnUntrackedEvent;

		_charInputModel = charInputModel;
		_charInputModel.InputEvent += OnInputEvent;
	}

	public bool UnqueueFromCompletedTargetList(EntityModel entityModel)
	{
		return _completedTargetList.Remove(entityModel);
	}

	public EntityModel UnqueueFirstCompletedTarget()
	{
		if(_completedTargetList.Count > 0)
		{
			EntityModel model = _completedTargetList[0];
			UnqueueFromCompletedTargetList(model);
		}

		return null;
	}

	private void OnInputEvent(char c)
	{
		if(_currentTypingTarget == null || _currentTypingTarget.IsDestroyed || _currentTypingTarget.GetComponent<WordsHp>().IsDead)
		{
			EntityModel target = _targetFilter.GetFirst(
			(e) =>
			{
				if(e.IsDestroyed || e.GetComponent<WordsHp>().IsDead)
					return false;

				if(_completedTargetList.Contains(e))
					return false;

				if(!WordsHp.IsHit(c, e.GetComponent<WordsHp>().GetCurrentChar()))
					return false;

				return true;

			}, (a, b) =>
			{
				float distA = (a.ModelTransform.Position - Parent.GetComponent<ModelTransform>().Position).magnitude;
				float distB = (b.ModelTransform.Position - Parent.GetComponent<ModelTransform>().Position).magnitude;
				return (int)(distA - distB);
			});

			SetTypeTarget(target);
		}

		if(_currentTypingTarget != null)
		{
			WordsHp targetWordsHp = _currentTypingTarget.GetComponent<WordsHp>();
			char requiredChar = targetWordsHp.GetChar(_buildupShootString.Length);
			if(WordsHp.IsHit(c, requiredChar))
			{
				_buildupShootString += requiredChar;
				if(targetWordsHp.CurrentTargetWord.Length == _buildupShootString.Length)
				{
					CompleteCurrentTypingTarget();
				}
			}
		}
	}

	private void OnUntrackedEvent(EntityModel entity)
	{
		_completedTargetList.Remove(entity);
	}

	protected override void Removed()
	{
		if(_charInputModel != null)
		{
			_charInputModel.InputEvent -= OnInputEvent;
		}

		_charInputModel = null;

		_currentTypingTarget = null;
		_buildupShootString = null;

		_completedTargetList.Clear();
		_completedTargetList = null;
	}

	private void CompleteCurrentTypingTarget()
	{
		if(_currentTypingTarget != null)
		{
			EntityModel target = _currentTypingTarget;
			_completedTargetList.Add(target);
			SetTypeTarget(null);

			if(TargetCompletedEvent != null)
			{
				TargetCompletedEvent(target);
			}
		}
	}

	private void SetTypeTarget(EntityModel target)
	{
		EntityModel previousTarget = _currentTypingTarget;

		if(previousTarget == target)
			return;

		_buildupShootString = "";

		_currentTypingTarget = target;

		if(TargetSetEvent != null)
		{
			TargetSetEvent(_currentTypingTarget, previousTarget);
		}
	}
}
