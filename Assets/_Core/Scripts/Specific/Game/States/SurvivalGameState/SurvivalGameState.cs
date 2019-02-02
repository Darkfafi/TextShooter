using System;

namespace SurvivalGame
{
	public class SurvivalGameState : BaseGameState, IGame, IFactoryCreator
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

		public WordsList WordsList
		{
			get; private set;
		}

		private TargetingSystem _targetingSystem;

		protected override void OnSetupState()
		{
			ParentGame.Factories.RegisterFactoryCreator(this);

			SurvivalGameStateManager = new GameStateManager<SurvivalGameState>(this);

			// Settings
			WordsList = new WordsList(SessionSettings.Request<WordsListSettings>().WordsListDocumentText);

			// Input
			CharInputModel = new CharInputModel();

			// Setup Global Mechanics

			// -- Game -- \\
			_targetingSystem = new TargetingSystem(CharInputModel, ParentGame.GameCamera, ParentGame.TimekeeperModel);

			// -- UI -- \\
			WordsDisplayerModel = new WordsDisplayerModel(ParentGame.TimekeeperModel);
			TargetingWordItemModificationModel = new TargetingWordItemModificationModel(_targetingSystem.Targeting, WordsDisplayerModel);

			// Setup Player
			TurretModel = new TurretModel(ParentGame.TimekeeperModel);
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
			ParentGame.Factories.UnregisterFactoryCreator(this);

			SurvivalGameStateManager.Clean();

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

		public IFactory CreateFactoryIfAble(Type factoryType)
		{
			if(factoryType == typeof(EnemyFactory))
				return new EnemyFactory(ParentGame.TimekeeperModel, EnemyDatabaseParser.ParseXml(SessionSettings.Request<EnemySettings>().EnemyDatabaseString), WordsList);
			else if(factoryType == typeof(WeaponFactory))
				return new WeaponFactory(WeaponDatabaseParser.ParseXml(SessionSettings.Request<WeaponSettings>().WeaponDatabaseString));

			return null;
		}
	}
}