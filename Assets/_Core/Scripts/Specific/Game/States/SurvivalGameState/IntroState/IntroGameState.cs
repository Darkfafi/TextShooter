using System;

namespace SurvivalGame
{
	public class IntroGameState : SubGameState<SurvivalGameState, GameModel>
	{
		public enum IntroState
		{
			None = 0,
			CameraCinematic = 1,
			End = 2
		}

		public event Action<IntroState> IntroStateSwitchedEvent;

		private IntroState _currentState;
		private bool _turretPreActiveState;

		private void GoToNextState()
		{
			_currentState = _currentState + 1;

			switch(_currentState)
			{
				case IntroState.End:
					ParentGame.TurretModel.SetGunActiveState(_turretPreActiveState);
					ParentGame.StartGame();
					break;
				default:
					if(IntroStateSwitchedEvent != null)
					{
						IntroStateSwitchedEvent(_currentState);
					}
					MethodPermitter.ExecuteWhenPermitted((int)_currentState, GoToNextState);
					break;
			}
		}

		protected override void OnSetupState()
		{
			_currentState = IntroState.None;
			_turretPreActiveState = ParentGame.TurretModel.IsGunActive;
			ParentGame.TurretModel.SetGunActiveState(false);
		}

		protected override void OnStartState()
		{
			GoToNextState();
		}

		protected override void OnEndState()
		{

		}
	}
}
