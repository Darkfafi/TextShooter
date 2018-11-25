public class Controller : IAbstractController
{
	public static Controller Link(IModel model, IView view)
	{
		return new Controller(model, view);
	}

	private Controller(IModel model, IView view)
	{
		CoreModel = model;
		CoreView = view;
		CoreModel.SetupModel(this);
		CoreView.SetupView(this);
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

	public virtual void Destroy()
	{
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
	void Destroy();
}

public interface IMethodPermitter
{
	MethodPermitter MethodPermitter
	{
		get;
	}
}