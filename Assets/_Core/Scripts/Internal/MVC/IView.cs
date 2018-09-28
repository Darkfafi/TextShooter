public interface IView
{
    object LinkingController { get; }
    void SetupView(object controller);
    void DestroyView();
}
