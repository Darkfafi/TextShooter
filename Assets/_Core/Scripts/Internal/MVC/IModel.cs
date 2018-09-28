public interface IModel
{
    IDestroyable LinkingController { get; }
    MethodPermitter MethodPermitter { get; }
    void SetupModel(IDestroyable controller);
    void Destroy();
}
