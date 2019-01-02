using System;

public class TimelineEventProgressor
{
	public event Action<TimelineEventProgressor> GoalMatchedEvent;
	public event Action<TimelineEventProgressor> GoalUnmatchedEvent;

	public bool IsGoalMatched
	{
		get; private set;
	}

	public string ProgressorName
	{
		get; private set;
	}

	public int ProgressorType
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

	public TimelineEventProgressor(int goalValue)
	{
		GoalValue = goalValue;
	}

	public static bool AreAllGoalsMatched(params TimelineEventProgressor[] progressors)
	{
		for(int i = 0; i < progressors.Length; i++)
		{
			if(!progressors[i].IsGoalMatched)
				return false;
		}

		return true;
	}

	public static int GetAllCurrentValue(params TimelineEventProgressor[] progressors)
	{
		int value = 0;
		for(int i = 0; i < progressors.Length; i++)
		{
			value += progressors[i].CurrentValue;
		}
		return value;
	}

	public static int GetAllGoalValue(params TimelineEventProgressor[] progressors)
	{
		int value = 0;
		for(int i = 0; i < progressors.Length; i++)
		{
			value += progressors[i].GoalValue;
		}
		return value;
	}

	public static float GetAllCompletionNormalized(params TimelineEventProgressor[] progressors)
	{
		int currentValue = 0;
		int goalValue = 0;

		for(int i = 0; i < progressors.Length; i++)
		{
			currentValue += progressors[i].CurrentValue;
			goalValue += progressors[i].GoalValue;
		}

		if(goalValue == 0)
			return 0f;

		return currentValue / goalValue;
	}

	public float CompletionNormalized
	{
		get
		{
			if(GoalValue == 0)
				return 0f;

			return CurrentValue / GoalValue;
		}
	}

	public void UpdateValue(int value)
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
