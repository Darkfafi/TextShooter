public static class MVCUtil
{
	public static T GetView<T>(IModel model) where T : class, IView
	{
		return GetView<T>((Controller)model.LinkingController);
	}
	public static T GetView<T, M>(M model) where T : class, IView where M : class, IModel
	{
		return GetView<T>((Controller)model.LinkingController);
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
}
