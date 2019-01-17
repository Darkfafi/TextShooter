using System.Collections.Generic;
using UnityEngine;

public class Targeting
{
	public delegate void NewOldTargetHandler(EntityModel newTarget, EntityModel previousTarget);
	public delegate void CharTargetHandler(EntityModel target, char newChar, char requiredChar, int index);
	public delegate void TargetHandler(EntityModel target);
	public delegate void TargetingEnabledState(Targeting targeting, bool enabledState);
	public event NewOldTargetHandler TargetSetEvent;
	public event CharTargetHandler TargetCharTypedEvent;
	public event TargetHandler TargetCompletedEvent;
	public event TargetingEnabledState TargetingEnabledStateChangedEvent;

	public int CurrentShootIndex
	{
		get
		{
			if(string.IsNullOrEmpty(BuildupShootString))
			{
				return 0;
			}

			return BuildupShootString.Length;
		}
	}

	public string BuildupShootString
	{
		get; private set;
	}

	public EntityModel CurrentTypingTarget
	{
		get; private set;
	}

	public EntityFilter<EntityModel> TargetsFilter
	{
		get
		{
			return _targetsFilter;
		}
	}

	public Vector3 OriginPosition
	{
		get; private set;
	}
	
	public bool IsEnabled
	{
		get; private set;
	}

	private List<EntityModel> _completedTargetList = new List<EntityModel>();

	private CharInputModel _charInputModel;
	private EntityFilter<EntityModel> _targetsFilter;

	public Targeting(CharInputModel charInputModel, FilterRules filterRules)
	{
		IsEnabled = true;

		FilterRules.OpenConstructOnFilterRules(filterRules);
		FilterRules.AddComponentToConstruct<WordsHolder>(false);
		FilterRules.AddComponentToConstruct<WordsHp>(false);
		FilterRules.CloseConstruct(out filterRules);

		_targetsFilter = EntityFilter<EntityModel>.Create(filterRules);
		_targetsFilter.UntrackedEvent += OnUntrackedEvent;

		_charInputModel = charInputModel;
		_charInputModel.InputEvent += OnInputEvent;
	}

	~Targeting()
	{
		if(_charInputModel != null)
		{
			_charInputModel.InputEvent -= OnInputEvent;
		}

		_charInputModel = null;

		CurrentTypingTarget = null;
		BuildupShootString = null;

		_targetsFilter.UntrackedEvent -= OnUntrackedEvent;
		_targetsFilter.Clean();
		_targetsFilter = null;

		_completedTargetList.Clear();
		_completedTargetList = null;
	}

	public bool IsTargetCompleted(EntityModel target)
	{
		return _completedTargetList.Contains(target);
	}

	public void SetEnabledState(bool enabledState)
	{
		if(IsEnabled == enabledState)
			return;

		IsEnabled = enabledState;

		if(TargetingEnabledStateChangedEvent != null)
		{
			TargetingEnabledStateChangedEvent(this, enabledState);
		}
	}

	public List<EntityModel> GetAllTargets(bool includingCurrentTyping)
	{
		List<EntityModel> targetsToReturn = new List<EntityModel>();
		if(CurrentTypingTarget != null && includingCurrentTyping)
		{
			targetsToReturn.Add(CurrentTypingTarget);
		}
		targetsToReturn.AddRange(_completedTargetList);
		return targetsToReturn;
	}

	private void OnInputEvent(char c)
	{
		if(!IsEnabled)
			return;

		if(CurrentTypingTarget == null || CurrentTypingTarget.IsDestroyed || CurrentTypingTarget.GetComponent<WordsHp>().IsDead)
		{
			EntityModel target = _targetsFilter.GetFirst(
			(e) =>
			{
				if(e.IsDestroyed || e.GetComponent<WordsHp>().IsDead)
					return false;

				if(IsTargetCompleted(e))
					return false;

				if(!WordsHp.IsHit(c, e.GetComponent<WordsHp>().GetCurrentChar()))
					return false;

				return true;

			}, (a, b) =>
			{
				float distA = (a.ModelTransform.Position - OriginPosition).magnitude;
				float distB = (b.ModelTransform.Position - OriginPosition).magnitude;
				return (int)(distA - distB);
			});

			SetTypeTarget(target);
		}

		if(CurrentTypingTarget != null)
		{
			WordsHp targetWordsHp = CurrentTypingTarget.GetComponent<WordsHp>();
			int index = CurrentShootIndex;
			char requiredChar = targetWordsHp.GetChar(index);

			if(TargetCharTypedEvent != null)
			{
				TargetCharTypedEvent(CurrentTypingTarget, c, requiredChar, index);
			}

			if(WordsHp.IsHit(c, requiredChar))
			{
				BuildupShootString += requiredChar;
				if(targetWordsHp.CurrentTargetWord.Length == BuildupShootString.Length)
				{
					CompleteCurrentTypingTarget();
				}
			}
		}
	}

	public void SetOriginPosition(Vector3 position)
	{
		OriginPosition = position;
	}

	private void OnUntrackedEvent(EntityModel entity)
	{
		if(entity == null)
			return;

		_completedTargetList.Remove(entity);
		entity.GetComponent<WordsHolder>().WordCycledEvent -= OnWordCycledEvent;
		entity.GetComponent<WordsHp>().WordCharHitEvent -= OnWordCharHitEvent;
	}

	private void CompleteCurrentTypingTarget()
	{
		if(CurrentTypingTarget != null && !IsTargetCompleted(CurrentTypingTarget))
		{
			EntityModel target = CurrentTypingTarget;
			_completedTargetList.Add(target);
			target.GetComponent<WordsHolder>().WordCycledEvent += OnWordCycledEvent;
			target.GetComponent<WordsHp>().WordCharHitEvent += OnWordCharHitEvent;
			SetTypeTarget(null);

			if(TargetCompletedEvent != null)
			{
				TargetCompletedEvent(target);
			}
		}
	}

	private void OnWordCharHitEvent(string word, int charIndex, WordsHp hitter)
	{
		if(hitter.IsDead)
		{
			OnUntrackedEvent((EntityModel)hitter.Parent);
		}
	}

	private void OnWordCycledEvent(string previousWord, string newWord, WordsHolder holder)
	{
		OnUntrackedEvent((EntityModel)holder.Parent);
	}

	private void SetTypeTarget(EntityModel target)
	{
		EntityModel previousTarget = CurrentTypingTarget;

		if(previousTarget == target)
			return;

		BuildupShootString = "";

		CurrentTypingTarget = target;

		if(TargetSetEvent != null)
		{
			TargetSetEvent(CurrentTypingTarget, previousTarget);
		}
	}
}
