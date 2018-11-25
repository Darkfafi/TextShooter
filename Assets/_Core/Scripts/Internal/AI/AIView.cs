using System;
using System.Collections.Generic;
using UnityEngine;

public class AIView : BaseView
{
	private Dictionary<Type, AIStateView> _stateToViewLink = new Dictionary<Type, AIStateView>();
	private AIStateView _currentAIStateView;

	public void LinkAIStateWithView<T>(AIStateView stateView) where T : AIState
	{
		Type t = typeof(T);
		if(_stateToViewLink.ContainsKey(t))
		{
			Debug.LogErrorFormat("Can't link {0} to {1} for {1} already has a link", stateView.ToString(), t.ToString());
			return;
		}

		_stateToViewLink.Add(t, stateView);
	}


	protected override void OnViewReady()
	{
		MVCUtil.GetModel<AIModel>(this).StateSetEvent += OnStateSetEvent;
	}

	protected override void OnViewDestroy()
	{
		MVCUtil.GetModel<AIModel>(this).StateSetEvent -= OnStateSetEvent;
		OnStateSetEvent(null);
	}

	private void OnStateSetEvent(AIState newState)
	{
		if(_currentAIStateView != null)
		{
			_currentAIStateView.Unlink();
		}

		if(newState != null && _stateToViewLink.TryGetValue(newState.GetType(), out _currentAIStateView))
		{
			_currentAIStateView.LinkWithState(newState);
		}
	}
}

public abstract class AIStateView
{
	public AIState AIState
	{
		get; private set;
	}

	public void LinkWithState(AIState state)
	{
		AIState = state;
		OnLinkedWithState();
	}

	public void Unlink()
	{
		OnUnlink();
		AIState = null;
	}

	protected abstract void OnLinkedWithState();
	protected abstract void OnUnlink();
}
