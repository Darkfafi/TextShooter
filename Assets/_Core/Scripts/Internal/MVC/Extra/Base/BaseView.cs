using System;
using UnityEngine;

/// <summary>
/// The design is to have the View contain all the Unity / visual specific code and use the Model data to set its visual state.
/// It is to only read from the Model, not to alter its data.
/// </summary>
public abstract class MonoBaseView : MonoBehaviour, IView
{
	public Action<MonoBaseView> ViewDestroyedEvent;

	private BaseView _baseView = new BaseView();

	public IMethodPermitter LinkingController
	{
		get
		{
			if(_baseView == null)
				return null;

			return _baseView.LinkingController;
		}
	}

	public virtual void PreDestroyView()
	{
		if(_baseView == null)
			return;

		_baseView.PreDestroyView();
	}

	public virtual void DestroyView()
	{
		if(LinkingController == null)
		{
			return;
		}

		_baseView.DestroyView();

		if(ViewDestroyedEvent != null)
		{
			ViewDestroyedEvent(this);
		}
		
		OnViewDestroy();
		ViewDestruction();
		_baseView = null;
	}

	public virtual void PreSetupView(IMethodPermitter controller)
	{
		if(LinkingController != null)
			return;

		_baseView.PreSetupView(controller);
		OnPreViewReady();
	}

	public virtual void SetupView()
	{
		_baseView.SetupView();
		OnViewReady();
	}

	protected virtual void ViewDestruction()
	{
		Destroy(gameObject);
	}

	private void OnDestroy()
	{
		DestroyView();
	}

	protected virtual void OnPreViewReady()
	{
	}
	protected virtual void OnViewReady()
	{
	}
	protected virtual void OnViewDestroy()
	{
	}
}

/// <summary>
/// The design is to have the View contain all the Unity / visual specific code and use the Model data to set its visual state.
/// </summary>
public class BaseView : IView
{
	public Action<BaseView> ViewDestroyedEvent;

	public IMethodPermitter LinkingController
	{
		get; private set;
	}

	public virtual void PreDestroyView()
	{

	}

	public virtual void PreSetupView(IMethodPermitter controller)
	{
		if(LinkingController != null)
			return;

		LinkingController = controller;
		OnPreViewReady();
	}

	public virtual void DestroyView()
	{
		if(LinkingController == null)
		{
			return;
		}

		OnViewDestroy();

		if(ViewDestroyedEvent != null)
		{
			ViewDestroyedEvent(this);
		}

		LinkingController = null;
	}

	public virtual void SetupView()
	{
		OnViewReady();
	}

	protected virtual void OnPreViewReady()
	{
	}
	protected virtual void OnViewReady()
	{
	}
	protected virtual void OnViewDestroy()
	{
	}
}