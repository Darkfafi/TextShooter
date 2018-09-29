public interface IView
{
    IMethodPermitter LinkingController { get; }
    void SetupView(IMethodPermitter controller);
    void DestroyView();
}
