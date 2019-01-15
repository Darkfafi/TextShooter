using System;

public abstract class BaseTimelineEventProgressor
{
	public delegate void ProgressorValueHandler(BaseTimelineEventProgressor progressor, int oldValue);
	public event Action<BaseTimelineEventProgressor> GoalMatchedEvent;
	public event ProgressorValueHandler ProgressorValueUpdatedEvent;
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

	public abstract void StartProgressor(string optionalValueStrings);
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
		int oldValue = CurrentValue;
		CurrentValue = value;

		if(ProgressorValueUpdatedEvent != null)
		{
			ProgressorValueUpdatedEvent(this, oldValue);
		}

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
