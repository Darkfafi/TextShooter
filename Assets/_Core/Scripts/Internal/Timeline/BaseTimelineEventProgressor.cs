using System;

public abstract class BaseTimelineEventProgressor
{
	public event Action<BaseTimelineEventProgressor> GoalMatchedEvent;
	public event Action<BaseTimelineEventProgressor> GoalUnmatchedEvent;

	public bool IsGoalMatched
	{
		get; private set;
	}

	public string ProgressorName
	{
		get; private set;
	}

	public int GoalValue
	{
		get; private set;
	}

	public int CurrentValue
	{
		get; private set;
	}

	public BaseTimelineEventProgressor(string progressorName, int goalValue)
	{
		ProgressorName = progressorName;
		GoalValue = goalValue;
	}

	public abstract void StartProgressor();
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
