namespace SurvivalGame
{
	public class SurvivalGameState : BaseGameState, IGame
	{
		public TurretModel TurretModel
		{
			get; private set;
		}

		public WordsDisplayerModel WordsDisplayerModel
		{
			get; private set;
		}

		public GameStateManager<SurvivalGameState> SurvivalGameStateManager
		{
			get; private set;
		}

		protected override void OnSetupState()
		{
			SurvivalGameStateManager = new GameStateManager<SurvivalGameState>(this);

			// Setup UI
			WordsDisplayerModel = new WordsDisplayerModel(Game.TimekeeperModel);

			// Setup Player
			TurretModel = new TurretModel(Game.TimekeeperModel);
		}

		protected override void OnStartState()
		{
			SurvivalGameStateManager.SetGameState<IntroGameState>();
		}

		protected override void OnEndState()
		{

		}
	}
}