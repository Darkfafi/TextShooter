using UnityEngine;

public class MovementState : StateMachineState<EntityModel>
{
	private TopDownMovement _affectingTopDownComponent;

	private Vector2 _locationToMoveTo;
	private EntityModel _entityToFollow;
	private float _minDistance;

	public void Setup(Vector2 locationToMoveTo)
	{
		_locationToMoveTo = locationToMoveTo;
	}

	public void Setup(EntityModel entityToFollow, float minDistance)
	{
		_minDistance = minDistance;
		_entityToFollow = entityToFollow;
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

public class MovementStateRequest : BaseStateMachineStateRequest<MovementState, EntityModel>
{
	private Vector2 _locationToMoveTo;
	private EntityModel _entityToFollow;
	private float _minDistance;

	public MovementStateRequest(Vector2 locationToMoveTo)
	{
		_locationToMoveTo = locationToMoveTo;
	}

	public MovementStateRequest(EntityModel entityToFollow, float minDistance)
	{
		_entityToFollow = entityToFollow;
		_minDistance = minDistance;
	}

	protected override void SetupCreatedState(MovementState state)
	{
		if(_entityToFollow == null)
		{
			state.Setup(_locationToMoveTo);
		}
		else
		{
			state.Setup(_entityToFollow, _minDistance);
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