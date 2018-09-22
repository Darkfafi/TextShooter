using UnityEngine;

public abstract class EntityView : MonoBehaviour, IView
{
    public Controller Controller
    {
        get; private set;
    }

    public virtual void DestroyView()
    {
        Controller = null;
    }

    public virtual void SetupView(Controller controller)
    {
        Controller = controller;
        OnViewReady();
    }

    private void OnDestroy()
    {
        OnViewDestroy();

        if (Controller != null)
        {
            Controller.Destroy();
        }
    }

    protected virtual void OnViewReady() { }
    protected virtual void OnViewDestroy() { }
}
