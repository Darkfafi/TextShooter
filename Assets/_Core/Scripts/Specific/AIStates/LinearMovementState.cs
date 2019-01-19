using UnityEngine;

/// <summary>
/// State which can make the affected entity move and follow in a linear manner.
/// </summary>
public class LinearMovementState : StateMachineState<EntityModel>
{
	private TopDownMovement _affectingTopDownComponent;

	private Vector2 _locationToMoveTo;
	private EntityModel _entityToFollow;
	private float _minDistance;
	private float? _speed;

	public void Setup(Vector2 locationToMoveTo)
	{
		_locationToMoveTo = locationToMoveTo;
	}

	public void Setup(EntityModel entityToFollow, float minDistance)
	{
		_minDistance = minDistance;
		_entityToFollow = entityToFollow;
	}

	public void SetSpecifiedSpeed(float speed)
	{
		_speed = speed;
	}

	protected override void OnActivated()
	{
		if(Affected.HasComponent<TopDownMovement>())
		{
			_affectingTopDownComponent = Affected.GetComponent<TopDownMovement>();
			_affectingTopDownComponent.StoppedMovingEvent += OnStoppedMovingEvent;

			if(_entityToFollow != null)
			{
				_affectingTopDownComponent.Follow(_entityToFollow.ModelTransform, _minDistance);
			}
			else
			{
				_affectingTopDownComponent.MoveTo(_locationToMoveTo);
			}

			if(_speed.HasValue)
			{
				_affectingTopDownComponent.SetMovementSpeed(_speed.Value);
			}
		}
		else
		{
			EndStateInternally();
		}
	}

	protected override void OnDeactivated()
	{
		_affectingTopDownComponent.StoppedMovingEvent -= OnStoppedMovingEvent;
		_affectingTopDownComponent.StopFollow();
		_affectingTopDownComponent = null;
	}

	private void OnStoppedMovingEvent(TopDownMovement component)
	{
		EndStateInternally();
	}
}

public class MovementStateRequest : BaseStateMachineStateRequest<LinearMovementState, EntityModel>
{
	private Vector2 _locationToMoveTo;
	private EntityModel _entityToFollow;
	private float _minDistance;
	private float? _speed;

	public MovementStateRequest(Vector2 locationToMoveTo)
	{
		_locationToMoveTo = locationToMoveTo;
	}

	public MovementStateRequest(EntityModel entityToFollow, float minDistance)
	{
		_entityToFollow = entityToFollow;
		_minDistance = minDistance;
	}

	public void SpecifySpeed(float speed)
	{
		_speed = speed;
	}

	public void UnsetSpecifiedSpeed()
	{
		_speed = null;
	}

	protected override void SetupCreatedState(LinearMovementState state)
	{
		if(_entityToFollow == null)
		{
			state.Setup(_locationToMoveTo);
		}
		else
		{
			state.Setup(_entityToFollow, _minDistance);
		}

		if(_speed.HasValue)
		{
			state.SetSpecifiedSpeed(_speed.Value);
		}
	}

	public override void Clean()
	{
		_entityToFollow = null;
	}

	public override bool IsAllowedToCreate()
	{
		if(_entityToFollow == null || (_entityToFollow != null && !_entityToFollow.IsDestroyed))
			return true;

		return false;
	}
}