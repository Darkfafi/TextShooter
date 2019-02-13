using UnityEngine;

namespace GameEditor
{
	public class GridDrawer : MonoBehaviour
	{
		public float Width
		{
			get
			{
				return _sizeX * _spacing;
			}
		}

		public float Height
		{
			get
			{
				return _sizeY * _spacing;
			}
		}

		[Header("Options")]
		[SerializeField]
		private Color _lineColor;

		[SerializeField]
		private float _lineThickness = 1f;

		[SerializeField]
		private int _specialLinePer = 5;

		[SerializeField]
		private Color _specialLineColor;

		[SerializeField]
		private float _specialLineThicknessScaler = 1.5f;

		[SerializeField]
		private int _sizeX = 250;

		[SerializeField]
		private int _sizeY = 250;

		[SerializeField]
		private int _spacing = 10;

		private Sprite _gridLineSprite;

		protected void Awake()
		{
			_gridLineSprite = Sprite.Create(Texture2D.whiteTexture, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f));

			for(int x = 0; x < _sizeY; x++)
			{
				CreateLine(true, x, _sizeY, Width);
			}

			for(int y = 0; y < _sizeX; y++)
			{
				CreateLine(false, y, _sizeX, Height);
			}

			gameObject.name = gameObject.name + string.Format("({0})", _sizeX * _sizeY);
		}

		private void CreateLine(bool horizontal, int index, int maxSize, float lineSize)
		{
			bool specialLine = _specialLinePer > 0 && index % _specialLinePer == 0;
			SpriteRenderer line = new GameObject(string.Format("Line {0}: {1}", (horizontal ? "Horizontal" : "Vertical"), index)).AddComponent<SpriteRenderer>();
			line.sprite = _gridLineSprite;
			Vector3 s = new Vector3(100 * _lineThickness, 1);
			s.x = specialLine ? s.x * _specialLineThicknessScaler : s.x;
			s.y = lineSize * 100;
			line.transform.localScale = s;

			if(horizontal)
			{
				line.transform.Rotate(0, 0, 90);
			}

			line.transform.SetParent(transform, true);
			int pos = (_spacing * index) - (_spacing * maxSize / 2);
			line.transform.localPosition = new Vector3(horizontal ? 0 : pos, !horizontal ? 0 : pos);
			line.color = specialLine ? _specialLineColor : _lineColor;
		}
	}
}