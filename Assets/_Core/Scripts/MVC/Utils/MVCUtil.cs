﻿public static class MVCUtil
{
    public static T GetView<T>(IModel model) where T : class, IView
    {
        return GetView<T>(model.Controller);
    }
    public static T GetView<T, M>(M model) where T : class, IView<M> where M : class, IModel
    {
        return GetView<T>(model.Controller);
    }

    public static T GetView<T>(Controller controller) where T : class, IView
    {
        return controller.CoreView as T;
    }

    public static T GetView<T, M>(Controller controller) where T : class, IView<M> where M : class, IModel
    {
        return controller.CoreView as T;
    }

    public static T GetModel<T>(IView view) where T : class, IModel
    {
        return view.CoreModel as T;
    }

    public static T GetModel<T>(Controller controller) where T : class, IModel
    {
        return controller.CoreModel as T;
    }
}
