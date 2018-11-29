using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace SurvivalGame
{
	public class IntroGameStateView : MonoGameStateView<SurvivalGameState>
	{
		[Header("Options")]
		[SerializeField]
		private float _startCameraZoom;

		[SerializeField]
		private float _introDurationInSeconds = 3f;

		[Header("Requirements")]
		[SerializeField]
		private CameraView _cameraForIntro;

		[SerializeField]
		private Image _overlayImage;

		private float _originalGameZoom;

		private IntroGameState _introGameState;
		private System.Action _cameraCinematicStatePermissionSignaller;

		protected override void OnPreStartStateView()
		{
			_introGameState = GameState as IntroGameState;
			_introGameState.MethodPermitter.BlockPermission((int)IntroGameState.IntroState.CameraCinematic, out _cameraCinematicStatePermissionSignaller);
			_originalGameZoom = _cameraForIntro.GetOrthographicSize();
			_introGameState.IntroStateSwitchedEvent += OnIntroStateSwitchedEvent;
		}

		protected override void OnStartStateView()
		{

		}

		protected override void OnEndStateView()
		{
			_introGameState.IntroStateSwitchedEvent -= OnIntroStateSwitchedEvent;
			_introGameState = null;
			_cameraCinematicStatePermissionSignaller = null;
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
			_cameraForIntro.SetCameraChainedToView(false);
			_cameraForIntro.Camera.orthographicSize = _startCameraZoom;
			Color c = _overlayImage.color;
			c.a = 1f;
			_overlayImage.color = c;

			// Camera Visual Animation
			_cameraForIntro.Camera.DOOrthoSize(_originalGameZoom, _introDurationInSeconds * 0.6f).SetEase(Ease.OutCubic).SetDelay(_introDurationInSeconds * 0.1f);
			_overlayImage.DOFade(0f, _introDurationInSeconds).OnComplete(() =>
			{
				_cameraForIntro.SetCameraChainedToView(true);
				_cameraCinematicStatePermissionSignaller();
			});
		}
	}
}