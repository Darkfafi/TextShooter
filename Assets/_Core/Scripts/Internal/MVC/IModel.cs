public interface IModel
{
    IDestroyable LinkingController { get; }
    void SetupModel(IDestroyable controller);
    void Destroy();
}
