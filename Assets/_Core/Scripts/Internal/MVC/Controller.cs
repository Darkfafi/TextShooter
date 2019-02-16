using System;

public class Controller : IAbstractController
{
	public event Action<IAbstractController> ControllerSetupEvent;

	public static Controller Link(IModel model, IView view)
	{
		return new Controller(model, view);
	}

	public static bool TryLink(IModel model, IView view)
	{
		Controller c;
		return TryLink(model, view, out c);
	}

	public static bool TryLink(IModel model, IView view, out Controller controller)
	{
		if(MVCUtil.HasView(model))
		{
			controller = model.LinkingController as Controller;
			return false;
		}

		controller = Link(model, view);
		return true;
	}

	private Controller(IModel model, IView view)
	{
		CoreModel = model;
		CoreView = view;

		CoreView.PreSetupView(this);
		CoreModel.SetupModel(this);
		CoreView.SetupView();

		if(ControllerSetupEvent != null)
		{
			ControllerSetupEvent(this);
		}
	}

	public IModel CoreModel
	{
		get; private set;
	}

	public IView CoreView
	{
		get; private set;
	}

	public MethodPermitter MethodPermitter
	{
		get
		{
			if(CoreModel == null)
				return null;

			return CoreModel.MethodPermitter;
		}
	}

	public bool HasView()
	{
		return CoreView != null;
	}

	public virtual void Destroy()
	{
		if(CoreView != null)
		{
			CoreView.PreDestroyView();
		}

		if(CoreModel != null)
		{
			CoreModel.Destroy();
		}

		if(CoreView != null)
		{
			CoreView.DestroyView();
		}

		CoreModel = null;
		CoreView = null;
	}
}


public interface IAbstractController : IMethodPermitter
{
	event Action<IAbstractController> ControllerSetupEvent;
	bool HasView();
	void Destroy();
}

public interface IMethodPermitter
{
	MethodPermitter MethodPermitter
	{
		get;
	}
}