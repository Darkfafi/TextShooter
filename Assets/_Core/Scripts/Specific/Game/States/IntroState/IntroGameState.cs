using System;

public class IntroGameState : BaseGameState
{
    public enum IntroState
    {
        None = 0,
        CameraCinematic = 1,
        End = 2
    }

    public event Action<IntroState> IntroStateSwitchedEvent;

    private IntroState _currentState;

    private void GoToNextState()
    {
        _currentState = _currentState + 1;

        switch(_currentState)
        {
            case IntroState.End:
                GameStateManager.SetGameState<SurvivalGameState>();
                break;
            default:
                if (IntroStateSwitchedEvent != null)
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
    }

    protected override void OnStartState()
    {
        GoToNextState();
    }

    protected override void OnEndState()
    {

    }
}
