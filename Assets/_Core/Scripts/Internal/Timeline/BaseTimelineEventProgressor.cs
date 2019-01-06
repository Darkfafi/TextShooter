using System;

public abstract class BaseTimelineEventProgressor
{
	public event Action<BaseTimelineEventProgressor> GoalMatchedEvent;
	public event Action<BaseTimelineEventProgressor> GoalUnmatchedEvent;

	public bool IsGoalMatched
	{
		get; private set;
	}

	public abstract string ProgressorName
	{
		get;
	}

	public int GoalValue
	{
		get; protected set;
	}

	public int CurrentValue
	{
		get; private set;
	}

	public BaseTimelineEventProgressor(int goalValue)
	{
		GoalValue = goalValue;
	}

	public abstract void StartProgressor(string optionalValueString);
	public abstract void EndProgressor();

	public float CompletionNormalized
	{
		get
		{
			if(GoalValue == 0)
				return 0f;

			return CurrentValue / GoalValue;
		}
	}

	protected void UpdateValue(int value)
	{
		CurrentValue = value;
		if(CurrentValue >= GoalValue)
		{
			if(!IsGoalMatched)
			{
				IsGoalMatched = true;
				if(GoalMatchedEvent != null)
				{
					GoalMatchedEvent(this);
				}
			}
		}
		else if(IsGoalMatched)
		{
			if(GoalUnmatchedEvent != null)
			{
				GoalUnmatchedEvent(this);
			}
		}
	}
}
