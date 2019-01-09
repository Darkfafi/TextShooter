public class TimeProgressor : BaseTimelineEventProgressor
{
	public override string ProgressorName
	{
		get
		{
			return TimelineSpecificGlobals.PROGRESSOR_NAME_TIME;
		}
	}

	private TimekeeperModel _timekeeperModel;
	private float _secondCounter = 0f;

	public TimeProgressor(TimekeeperModel timekeeperModel, int endTimeInSeconds) : base(endTimeInSeconds)
	{
		_timekeeperModel = timekeeperModel;
	}

	~TimeProgressor()
	{
		_timekeeperModel = null;
	}

	public override void StartProgressor(string optionalValueString)
	{
		int value = 0;
		string[] result = optionalValueString.Split('+');
		string parseableValue = result.Length > 0 ? result[result.Length - 1] : "";
		if(int.TryParse(parseableValue, out value))
		{
			if(result.Length <= 1)
			{
				GoalValue = value;
			}
			else
			{
				GoalValue += value;
			}
		}

		if(GoalValue <= 0)
		{
			UpdateValue(-1);
		}

		if(CurrentValue < 0) // No time given, end immediately.
			UpdateValue(CurrentValue + 1);
		else
			_timekeeperModel.ListenToFrameTick(OnUpdate);
	}

	private void OnUpdate(float deltaTime, float timeScale)
	{
		_secondCounter += deltaTime * timeScale;
		if(_secondCounter >= 1f)
		{
			_secondCounter -= 1f;
			UpdateValue(CurrentValue + 1);
		}
	}

	public override void EndProgressor()
	{
		_timekeeperModel.UnlistenFromFrameTick(OnUpdate);
	}
}
