namespace SurvivalGame
{
	public class SurvivalGameState : BaseGameState, IGame
	{
		public TurretModel TurretModel
		{
			get; private set;
		}

		public CharInputModel CharInputModel
		{
			get; private set;
		}

		public WordsDisplayerModel WordsDisplayerModel
		{
			get; private set;
		}

		public TargetingWordItemModificationModel TargetingWordItemModificationModel
		{
			get; private set;
		}

		public GameStateManager<SurvivalGameState> SurvivalGameStateManager
		{
			get; private set;
		}

		private TargetingSystem _targetingSystem;

		protected override void OnSetupState()
		{
			SurvivalGameStateManager = new GameStateManager<SurvivalGameState>(this);

			// Input
			CharInputModel = new CharInputModel();

			// Setup Global Mechanics

			// -- Game -- \\
			_targetingSystem = new TargetingSystem(CharInputModel, Game.GameCamera, Game.TimekeeperModel);

			// -- UI -- \\
			WordsDisplayerModel = new WordsDisplayerModel(Game.TimekeeperModel);
			TargetingWordItemModificationModel = new TargetingWordItemModificationModel(_targetingSystem.Targeting, WordsDisplayerModel);

			// Setup Player
			TurretModel = new TurretModel(Game.TimekeeperModel);
		}

		public void StartGame()
		{
			SurvivalGameStateManager.SetGameState<CampaignGameState>();
		}

		protected override void OnStartState()
		{
			SurvivalGameStateManager.SetGameState<IntroGameState>();
		}

		protected override void OnEndState()
		{
			// Input
			CharInputModel.Destroy();
			CharInputModel = null;

			// Setup Global Mechanics

			// -- Game -- \\
			_targetingSystem.Clean();
			_targetingSystem = null;

			// -- UI -- \\
			WordsDisplayerModel.Destroy();
			WordsDisplayerModel = null;

			TargetingWordItemModificationModel.Destroy();
			TargetingWordItemModificationModel = null;

			// Setup Player
			TurretModel.Destroy();
			TurretModel = null;
		}
	}
}