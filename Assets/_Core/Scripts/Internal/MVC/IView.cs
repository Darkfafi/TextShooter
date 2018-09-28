public interface IView
{
    object LinkingController { get; }
    MethodPermitter MethodPermitter { get; }
    void SetupView(object controller);
    void DestroyView();
}
