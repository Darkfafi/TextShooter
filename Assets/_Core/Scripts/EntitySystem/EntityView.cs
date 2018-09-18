using UnityEngine;

public abstract class EntityView<T> : EntityView, IView<T> where T : EntityModel
{
    public T Model
    {
        get
        {
            return (T)CoreModel;
        }
    }
}

public abstract class EntityView : MonoBehaviour, IView
{
    public IModel CoreModel
    {
        get; private set;
    }

    public virtual void Destroy()
    {
        CoreModel = null;
    }

    public virtual bool LinkModel(IModel model)
    {
        if (model != null)
        {
            CoreModel = model;
            return true;
        }
        else
        {
            CoreModel = null;
            return false;
        }
    }

    private void OnDestroy()
    {
        if(CoreModel != null)
        {
            CoreModel.Controller.Destroy();
        }
    }
}
