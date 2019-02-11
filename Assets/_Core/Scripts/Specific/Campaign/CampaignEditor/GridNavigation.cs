using UnityEngine;

[RequireComponent(typeof(Camera))]
public class GridNavigation : MonoBehaviour
{
	[Header("-- Options --")]

	[Header("Navigation")]
	[SerializeField]
	private float _navigationSpeed = 0.235f;

	[SerializeField]
	private float _dragReleaseSpeedMultiplier = 0.25f;

	[SerializeField]
	private float _velocityReduction = 0.15f;

	[Header("Zooming")]
	[SerializeField]
	private float MinZoom = 50f;

	[SerializeField]
	private float MaxZoomMaxValue = 300f;

	[SerializeField]
	private float _scrollSpeed = 10f;

	[SerializeField]
	private float _scrollOrthographicThreshold = 250f;

	[Header("-- Requirements-- ")]
	[SerializeField]
	private GridDrawer _gridDrawer;

	private float MaxZoom;
	private float Left;
	private float Right;
	private float Top;
	private float Bottom;

	private Camera _camera;

	private float _currentVelocity;
	private Vector2 _directionNormalized = new Vector2();

	protected void Awake()
	{
		_camera = GetComponent<Camera>();
		Refresh();
	}

	protected void OnGUI()
	{
		if(Event.current == null)
			return;

		if(Event.current.button == 2)
		{
			Vector2 delta = Event.current.delta;
			delta.x = -delta.x;

			if(Event.current.type == EventType.MouseDrag)
			{
				_currentVelocity = 0;
				_camera.transform.Translate(delta * _navigationSpeed * _camera.orthographicSize * Time.unscaledDeltaTime);
			}

			if(Event.current.type == EventType.MouseUp)
			{
				_directionNormalized = delta * _dragReleaseSpeedMultiplier;
				_currentVelocity = _navigationSpeed * _camera.orthographicSize;
			}
		}

		if(Event.current.isScrollWheel)
		{
			_camera.orthographicSize = Mathf.Clamp(_camera.orthographicSize + Event.current.delta.y * _scrollSpeed * (_camera.orthographicSize / _scrollOrthographicThreshold), MinZoom, MaxZoom);
			RefreshBounds();
		}

		_camera.transform.Translate(_directionNormalized * _currentVelocity * Time.unscaledDeltaTime);
		_currentVelocity = Mathf.Max(_currentVelocity - _velocityReduction * _camera.orthographicSize * Time.unscaledDeltaTime, 0);
		_camera.transform.position = new Vector3(Mathf.Clamp(_camera.transform.position.x, Left, Right), Mathf.Clamp(_camera.transform.position.y, Bottom, Top), _camera.transform.position.z);
		RefreshBounds();
	}

	// Max bounds calculations found on internet. 
	private void Refresh()
	{
		//calculate current screen ratio
		float w = Screen.width / _gridDrawer.Width;
		float h = Screen.height / _gridDrawer.Height;
		float ratio = w / h;
		float ratio2 = h / w;

		if(ratio2 > ratio)
		{
			MaxZoom = (_gridDrawer.Height / 2);
		}
		else
		{
			MaxZoom = (_gridDrawer.Height / 2);
			MaxZoom /= ratio;
		}

		MaxZoom = Mathf.Min(MaxZoomMaxValue, MaxZoom);

		RefreshBounds();
	}

	private void RefreshBounds()
	{
		var vertExtent = _camera.orthographicSize;
		var horzExtent = vertExtent * Screen.width / Screen.height;

		Left = horzExtent - _gridDrawer.Width / 2.0f + _gridDrawer.transform.position.x;
		Right = _gridDrawer.Width / 2.0f - horzExtent + _gridDrawer.transform.position.x;
		Bottom = vertExtent - _gridDrawer.Height / 2.0f + _gridDrawer.transform.position.y;
		Top = _gridDrawer.Height / 2.0f - vertExtent + _gridDrawer.transform.position.y;
	}
}
