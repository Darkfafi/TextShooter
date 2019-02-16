using System;

public static class MVCUtil
{
	public static T GetView<T>(IModel model) where T : class, IView
	{
		return GetView<T>((Controller)model.LinkingController);
	}

	public static void GetView<T>(BaseModel model, Action<T> callback) where T : class, IView
	{
		MVCUtilViewWaitProcess<T>.Create(callback, model);
	}

	public static T GetView<T, M>(M model) where T : class, IView where M : class, IModel
	{
		return GetView<T>((Controller)model.LinkingController);
	}

	public static bool HasView(Controller controller)
	{
		return controller != null && controller.CoreView != null;
	}

	public static bool HasView(IModel model)
	{
		return HasView((Controller)model.LinkingController);
	}

	public static T GetView<T>(Controller controller) where T : class, IView
	{
		return controller.CoreView as T;
	}

	public static T GetView<T, M>(Controller controller) where T : class, IView where M : class, IModel
	{
		return controller.CoreView as T;
	}

	public static T GetModel<T>(IView view) where T : class, IModel
	{
		return ((Controller)view.LinkingController).CoreModel as T;
	}

	public static T GetModel<T>(Controller controller) where T : class, IModel
	{
		return controller.CoreModel as T;
	}

	public static bool HasModel(Controller controller)
	{
		return controller != null && controller.CoreModel != null;
	}

	public static bool HasModel(IView view)
	{
		return HasModel((Controller)view.LinkingController);
	}

	private class MVCUtilViewWaitProcess<T> where T : class, IView
	{
		private Action<T> _callback;

		public static MVCUtilViewWaitProcess<T> Create(Action<T> callback, BaseModel model)
		{
			return new MVCUtilViewWaitProcess<T>(callback, model);
		}

		private MVCUtilViewWaitProcess(Action<T> callback, BaseModel model)
		{
			_callback = callback;
			model.ModelReadyEvent += OnModelReady;
			if(model.LinkingController != null && !model.LinkingController.HasView())
			{
				OnModelReady(model);
			}
			else if(model.LinkingController != null && model.LinkingController.HasView())
			{
				OnControllerSetup(model.LinkingController);
			}
		}

		private void OnModelReady(BaseModel model)
		{
			model.ModelReadyEvent -= OnModelReady;
			model.LinkingController.ControllerSetupEvent += OnControllerSetup;
		}

		private void OnControllerSetup(IAbstractController controller)
		{
			controller.ControllerSetupEvent -= OnControllerSetup;
			_callback(GetView<T>((Controller)controller));
			_callback = null;
		}
	}
}
