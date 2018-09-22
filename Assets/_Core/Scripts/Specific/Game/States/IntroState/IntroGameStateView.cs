using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class IntroGameStateView : MonoBehaviourGameStateView
{
    [SerializeField]
    private Camera _cameraForIntro;

    [SerializeField]
    private Image _overlayImage;

    [SerializeField]
    private float _startCameraZoom;

    [SerializeField]
    private float _introDurationInSeconds = 3f;

    private float _originalGameZoom;

    private IntroGameState _introGameState;

    protected override void Awake()
    {
        base.Awake();
        _originalGameZoom = _cameraForIntro.orthographicSize;
    }

    protected override void OnPreStartStateView()
    {
        _introGameState = GameState as IntroGameState;
        _introGameState.IntroStateSwitchedEvent += OnIntroStateSwitchedEvent;
    }

    protected override void OnStartStateView()
    {

    }

    protected override void OnEndStateView()
    {
        _introGameState.IntroStateSwitchedEvent -= OnIntroStateSwitchedEvent;
        _introGameState = null;
    }

    private void OnIntroStateSwitchedEvent(IntroGameState.IntroState newIntroState)
    {
        switch(newIntroState)
        {
            case IntroGameState.IntroState.CameraCinematic:
                DoCinematicCameraVisual();
                break;
        }
    }

    private void DoCinematicCameraVisual()
    {
        // Camera Visual Start Setup
        _cameraForIntro.orthographicSize = _startCameraZoom;
        Color c = _overlayImage.color;
        c.a = 1f;
        _overlayImage.color = c;

        // Camera Visual Animation
        _cameraForIntro.DOOrthoSize(_originalGameZoom, _introDurationInSeconds * 0.6f).SetEase(Ease.OutCubic).SetDelay(_introDurationInSeconds * 0.1f);
        _overlayImage.DOFade(0f, _introDurationInSeconds).OnComplete(() => {
            _introGameState.GoToNextState();
        });
    }
}
