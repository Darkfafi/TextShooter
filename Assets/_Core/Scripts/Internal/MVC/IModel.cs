public interface IModel
{
    Controller Controller { get; }
    void SetupModel(Controller controller);
    void Destroy();
}
