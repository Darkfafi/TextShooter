﻿public class TimeProgressor : BaseTimelineEventProgressor
{
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

	public override void StartProgressor()
	{
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
