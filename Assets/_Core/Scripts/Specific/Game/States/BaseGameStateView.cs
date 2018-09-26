public abstract class BaseGameStateView : IGameStateView<GameModel>
{
    public abstract void EndStateView();
    public abstract void PreStartStateView(GameState<GameModel> state);
    public abstract void StartStateView();
}
