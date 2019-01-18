using UnityEngine;

public class MovementState : StateMachineState<EntityModel>
{
	private TopDownMovement _affectingTopDownComponent;

	private Vector2 _locationToMoveTo;

	public void Setup(Vector2 locationToMoveTo)
	{
		_locationToMoveTo = locationToMoveTo;
	}

	protected override void OnActivated()
	{
		if(Affected.HasComponent<TopDownMovement>())
		{
			_affectingTopDownComponent = Affected.GetComponent<TopDownMovement>();
			_affectingTopDownComponent.MoveTo(_locationToMoveTo);
			_affectingTopDownComponent.ReachedDestinationEvent += OnReachedDestinationEvent;
		}
		else
		{
			EndStateInternally();
		}
	}

	private void OnReachedDestinationEvent(TopDownMovement component, Vector2 destination)
	{
		EndStateInternally();
	}

	protected override void OnDeactivated()
	{
		_affectingTopDownComponent.ReachedDestinationEvent -= OnReachedDestinationEvent;
		_affectingTopDownComponent = null;
	}
}

public class MovementStateRequest : BaseStateMachineStateRequest<MovementState, EntityModel>
{
	private Vector2 _locationToMoveTo;

	public MovementStateRequest(Vector2 locationToMoveTo)
	{
		_locationToMoveTo = locationToMoveTo;
	}

	protected override void SetupCreatedState(MovementState state)
	{
		state.Setup(_locationToMoveTo);
	}
}