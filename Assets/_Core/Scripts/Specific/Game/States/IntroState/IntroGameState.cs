using System;

public class IntroGameState : GameState
{
    public enum IntroState
    {
        None,
        CameraCinematic,
        End
    }

    public event Action<IntroState> IntroStateSwitchedEvent;

    private IntroState _currentState;

    public void GoToNextState()
    {
        _currentState = _currentState + 1;

        if (_currentState == IntroState.End)
        {
            UnityEngine.Debug.Log("GO TO GAME STATE!");
        }
        else
        {
            if (IntroStateSwitchedEvent != null)
            {
                IntroStateSwitchedEvent(_currentState);
            }
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
