using UnityEngine;

/// <summary>
/// Base View for the game. Recommended to be used with the EntityManager, but not required.
/// The design is to have the View contain all the Unity specific code and use the Model data to set its visual state.
/// </summary>
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
