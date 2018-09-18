public class Controller<M, V> : Controller where M : class, IModel where V : class, IView<M>
{
    public static Controller<M, V> Create(M model, V view)
    {
        Controller<M, V> c = new Controller<M, V>();
        c.Setup(model, view);
        return c;
    }

    public M Model
    {
        get
        {
            return (M)CoreModel;
        }
    }

    public V View
    {
        get
        {
            return (V)CoreView;
        }
    }
}

public class Controller
{
    public static Controller Create(IModel model, IView view)
    {
        Controller c = new Controller();
        c.Setup(model, view);
        return c;
    }

    protected void Setup(IModel model, IView view)
    {
        CoreModel = model;
        CoreView = view;
        CoreModel.SetupModel(this);
        CoreView.LinkModel(CoreModel);
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
            CoreView.LinkModel(null);
            CoreView.Destroy();
        }

        CoreModel = null;
        CoreView = null;
    }
}