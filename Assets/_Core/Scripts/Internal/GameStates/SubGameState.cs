public abstract class SubGameState<T, U> : GameState<T>, ISubGameState where T : GameState<U>, IGame where U : class, IGame
{
	public U MasterGame
	{
		get
		{
			ISubGameState subState = Game as ISubGameState;

			if(subState != null)
			{
				return (U)subState.MasterGame;
			}

			return Game == null ? null : Game.Game;
		}
	}

	IGame ISubGameState.MasterGame
	{
		get
		{
			return MasterGame;
		}
	}
}

public interface ISubGameState
{
	IGame MasterGame
	{
		get;
	}
}