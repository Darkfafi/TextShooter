public interface IView<M> : IView where M : class, IModel
{
    M Model { get; }
}

public interface IView
{
    IModel CoreModel { get; }
    bool LinkModel(IModel model);
    void Destroy();
}
