using Illusion.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Studio
{
	public class RoutePointComponent : MonoBehaviour
	{
		[SerializeField]
		private Image imageBack;

		[SerializeField]
		private TextMeshProUGUI _textName;

		[SerializeField]
		private CanvasGroup canvasGroup;

		[SerializeField]
		[Space]
		private GameObject _objAid;

		public Color color
		{
			set
			{
				imageBack.color = value;
			}
		}

		public string textName
		{
			set
			{
				_textName.text = value;
			}
		}

		public bool visible
		{
			get
			{
				return canvasGroup.alpha != 0f;
			}
			set
			{
				canvasGroup.Enable(value);
			}
		}

		public GameObject objAid
		{
			get
			{
				return _objAid;
			}
		}
	}
}
