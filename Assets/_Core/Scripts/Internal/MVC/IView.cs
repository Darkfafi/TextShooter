public interface IView
{
    Controller Controller { get; }
    void SetupView(Controller model);
    void DestroyView();
}
