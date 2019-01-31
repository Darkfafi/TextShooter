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
	private CameraModel _cameraModel;

	public Targeting(CharInputModel charInputModel, CameraModel cameraModel, FilterRules filterRules)
	{
		IsEnabled = true;

		_cameraModel = cameraModel;

		FilterRules.OpenConstructOnFilterRules(filterRules);
		FilterRules.AddComponentToConstruct<WordsLive>(false);
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

		_cameraModel = null;

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

		if(CurrentTypingTarget == null || CurrentTypingTarget.IsDestroyed || !CurrentTypingTarget.GetComponent<WordsLive>().Lives.IsAlive)
		{
			EntityModel target = _targetsFilter.GetFirst(
			(e) =>
			{
				if(e.IsDestroyed || !e.GetComponent<WordsLive>().Lives.IsAlive)
					return false;

				if(IsTargetCompleted(e))
					return false;

				if(!WordsHolder.IsCharMatch(e.GetComponent<WordsLive>().WordsHolder.GetChar(0), c))
					return false;

				if(_cameraModel.IsOutsideOfOrthographic(e.ModelTransform.Position))
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
			WordsLive wordsLive = CurrentTypingTarget.GetComponent<WordsLive>();
			int index = CurrentShootIndex;
			char requiredChar = wordsLive.WordsHolder.GetChar(index);

			if(TargetCharTypedEvent != null)
			{
				TargetCharTypedEvent(CurrentTypingTarget, c, requiredChar, index);
			}

			if(WordsHolder.IsCharMatch(c, requiredChar))
			{
				BuildupShootString += requiredChar;
				if(wordsLive.WordsHolder.CurrentWord.Length == BuildupShootString.Length)
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

		entity.GetComponent<WordsLive>().WordsHolder.WordCycledEvent -= OnWordCycledEvent;
		_completedTargetList.Remove(entity);

		if(CurrentTypingTarget == entity)
		{
			SetTypeTarget(null);
		}
	}

	private void CompleteCurrentTypingTarget()
	{
		if(CurrentTypingTarget != null && !IsTargetCompleted(CurrentTypingTarget))
		{
			EntityModel target = CurrentTypingTarget;
			_completedTargetList.Add(target);
			SetTypeTarget(null);
			target.GetComponent<WordsLive>().WordsHolder.WordCycledEvent += OnWordCycledEvent;

			if(TargetCompletedEvent != null)
			{
				TargetCompletedEvent(target);
			}
		}
	}

	private void OnWordCycledEvent(string previousWord, string newWord, WordsHolder holder)
	{
		OnUntrackedEvent((EntityModel)holder.Parent);
	}

	private void SetTypeTarget(EntityModel target)
	{
		EntityModel previousTarget = CurrentTypingTarget;

		if(previousTarget != null)
		{
			previousTarget.GetComponent<WordsLive>().WordsHolder.WordCycledEvent -= OnWordCycledEvent;
		}

		if(previousTarget == target && target != null)
			return;

		BuildupShootString = "";
		CurrentTypingTarget = target;

		if(CurrentTypingTarget != null)
			CurrentTypingTarget.GetComponent<WordsLive>().WordsHolder.WordCycledEvent += OnWordCycledEvent;

		if(previousTarget != target && TargetSetEvent != null)
		{
			TargetSetEvent(CurrentTypingTarget, previousTarget);
		}
	}
}
