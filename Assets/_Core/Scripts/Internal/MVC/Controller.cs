public class Controller : IDestroyable
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

public interface IDestroyable
{
    void Destroy();
}