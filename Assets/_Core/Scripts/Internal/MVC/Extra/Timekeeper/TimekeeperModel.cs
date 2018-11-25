public class TimekeeperModel : BaseModel
{
	public delegate void FrameTickHandler(float deltaTime, float timeScale);

	public float TimeScale = 1f;
	public double SecondsPassedSessionUnscaled
	{
		get; private set;
	}
	public double SecondsPassedSessionScaled
	{
		get; private set;
	}

	private FrameTickHandler _frameTickAction;

	public void FrameTick(float deltaTime)
	{
		SecondsPassedSessionUnscaled += deltaTime;
		SecondsPassedSessionScaled += deltaTime * TimeScale;

		if(_frameTickAction != null)
		{
			_frameTickAction(deltaTime, TimeScale);
		}
	}

	public void ListenToFrameTick(FrameTickHandler callback)
	{
		_frameTickAction += callback;
	}

	public void UnlistenFromFrameTick(FrameTickHandler callback)
	{
		_frameTickAction -= callback;
	}

	protected override void OnModelDestroy()
	{
		_frameTickAction = null;
	}
}
