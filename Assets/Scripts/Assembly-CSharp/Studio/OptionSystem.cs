using Config;
using UnityEngine;

namespace Studio
{
	public class OptionSystem : BaseSystem
	{
		public float cameraSpeedX = 1f;

		public float cameraSpeedY = 1f;

		public float cameraSpeed = 1f;

		public float manipulateSize = 1f;

		public float manipuleteSpeed = 1f;

		public int initialPosition;

		public int selectedState;

		public bool autoHide = true;

		public bool autoSelect;

		public int snap;

		public Color colorFKHair = Color.white;

		public Color colorFKNeck = Color.white;

		public Color colorFKBreast = Color.white;

		public Color colorFKBody = Color.white;

		public Color colorFKRightHand = Color.white;

		public Color colorFKLeftHand = Color.white;

		public Color colorFKSkirt = Color.white;

		public bool lineFK = true;

		public Color colorFKItem = Color.white;

		public int _logo;

		public float _routeLineWidth = 1f;

		public bool routePointLimit = true;

		public bool startupLoad;

		public int logo
		{
			get
			{
				return Mathf.Clamp(_logo, 0, 9);
			}
			set
			{
				_logo = Mathf.Clamp(value, 0, 9);
			}
		}

		public float routeLineWidth
		{
			get
			{
				return _routeLineWidth * 16f;
			}
			set
			{
				_routeLineWidth = value / 16f;
			}
		}

		public OptionSystem(string elementName)
			: base(elementName)
		{
		}

		public override void Init()
		{
			cameraSpeedX = 1f;
			cameraSpeedY = 1f;
			cameraSpeed = 1f;
			manipulateSize = 1f;
			manipuleteSpeed = 1f;
			initialPosition = 0;
			selectedState = 0;
			autoHide = true;
			autoSelect = false;
			snap = 0;
			colorFKHair = Color.white;
			colorFKNeck = Color.white;
			colorFKBreast = Color.white;
			colorFKBody = Color.white;
			colorFKRightHand = Color.white;
			colorFKLeftHand = Color.white;
			colorFKSkirt = Color.white;
			lineFK = true;
			colorFKItem = Color.white;
			_logo = 0;
			_routeLineWidth = 1f;
			routePointLimit = true;
			startupLoad = false;
		}

		public Color GetFKColor(int _idx)
		{
			switch (_idx)
			{
			case 0:
				return colorFKHair;
			case 1:
				return colorFKNeck;
			case 2:
				return colorFKBreast;
			case 3:
				return colorFKBody;
			case 4:
				return colorFKRightHand;
			case 5:
				return colorFKLeftHand;
			case 6:
				return colorFKSkirt;
			default:
				return Color.white;
			}
		}

		public void SetFKColor(int _idx, Color _color)
		{
			switch (_idx)
			{
			case 0:
				colorFKHair = _color;
				break;
			case 1:
				colorFKNeck = _color;
				break;
			case 2:
				colorFKBreast = _color;
				break;
			case 3:
				colorFKBody = _color;
				break;
			case 4:
				colorFKRightHand = _color;
				break;
			case 5:
				colorFKLeftHand = _color;
				break;
			case 6:
				colorFKSkirt = _color;
				break;
			}
		}
	}
}
