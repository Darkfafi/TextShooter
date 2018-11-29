namespace SurvivalGame
{
	public class WavesGameState : SubGameState<SurvivalGameState, GameModel>
	{
		public WaveSystemModel WaveSystem
		{
			get; private set;
		}

		protected override void OnSetupState()
		{
			// Setup Environment
			WaveSystem = new WaveSystemModel(MasterGame.GameCamera, MasterGame.TimekeeperModel);
		}

		protected override void OnStartState()
		{
			WaveSystem.StartWaveSystem();
		}

		protected override void OnEndState()
		{

		}
	}
}
