using UnityEngine;

public class TopDownMovement : BaseModelComponent
{
	/// <summary>
	/// The movement speed which is being used during movement
	/// </summary>
	public float MovementSpeed
	{
		get; private set;
	}
	/// <summary>
	/// The movement speed which is saved as base speed and used in `MoveTo` without specified speed parameter
	/// </summary>
	public float BaseSpeed
	{
		get; private set;
	}

	/// <summary>
	/// The speed at which the character rotates to the direction it is moving (Default 0.35f)
	/// </summary>
	public float RotationSpeed
	{
		get; private set;
	}

	/// <summary>
	/// The position the Transform was located when the `MoveTo` was called
	/// </summary>
	public Vector3 StartPosition
	{
		get; private set;
	}

	/// <summary>
	/// The position the Transform is moving towards, it is the target given to the `MoveTo` method
	/// </summary>
	public Vector3 CurrentTarget
	{
		get; private set;
	}

	/// <summary>
	/// The current progress between start and end position as normalized value
	/// </summary>
	public float CurrentNormalizedPosition
	{
		get; private set;
	}

	/// <summary>
	/// Returns whether the Transform is still performing its movement task (MovementSpeed > 0)
	/// </summary>
	public bool IsMoving
	{
		get
		{
			return MovementSpeed > 0f;
		}
	}

	private TimekeeperModel _timekeeperModel;
	private ModelTransform _transformToAffect;

	private Vector3 _delta;
	private float _duration;
	private float _timePassed;

	public void Setup(TimekeeperModel timekeeper, float baseSpeed = 4f)
	{
		_timekeeperModel = timekeeper;
		RotationSpeed = 0.35f;
		SetBaseSpeed(baseSpeed);
		SetMovementSpeed(0f);
		_timekeeperModel.ListenToFrameTick(Update);
	}

	protected override void Added()
	{
		_transformToAffect = Components.GetComponent<ModelTransform>();
	}

	protected override void Removed()
	{
		_timekeeperModel.UnlistenFromFrameTick(Update);
		_timekeeperModel = null;
		_transformToAffect = null;
	}

	public void MoveTo(Vector2 position)
	{
		MoveTo(position, BaseSpeed);
	}

	public void MoveTo(Vector2 position, float speed)
	{
		if(!IsEnabled)
			return;

		StartPosition = _transformToAffect.Position;
		CurrentTarget = position;
		_timePassed = 0f;
		MovementSpeed = speed;
		_delta = CurrentTarget - _transformToAffect.Position;
	}

	public void StopMoving()
	{
		SetMovementSpeed(0f);
	}

	public void SetBaseSpeed(float newSpeed)
	{
		BaseSpeed = newSpeed;
	}

	public void SetMovementSpeed(float newSpeed)
	{
		if(IsMoving)
		{
			MoveTo(CurrentTarget, newSpeed);
		}
		else
		{
			MovementSpeed = newSpeed;
		}
	}

	private void Update(float deltaTime, float timeScale)
	{
		if(IsEnabled && IsMoving && Parent != null && !Parent.IsDestroyed)
		{
			_timePassed += deltaTime * timeScale;
			_duration = _delta.magnitude / MovementSpeed;
			CurrentNormalizedPosition = Mathf.Clamp01(_timePassed / _duration);

			_transformToAffect.SetPos(StartPosition + (CurrentNormalizedPosition * _delta));
			_transformToAffect.SetRotZ(Mathf.LerpAngle(_transformToAffect.Rotation.z, Mathf.Atan2(_delta.y, _delta.x) * Mathf.Rad2Deg, RotationSpeed * timeScale));

			if(CurrentNormalizedPosition == 1f)
			{
				StopMoving();
			}
		}
	}
}
