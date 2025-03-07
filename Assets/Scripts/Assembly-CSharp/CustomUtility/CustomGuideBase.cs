using UnityEngine;
using UnityEngine.EventSystems;

namespace CustomUtility
{
	public class CustomGuideBase : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IEventSystemHandler
	{
		[SerializeField]
		protected Color colorNormal;

		[SerializeField]
		protected Color colorHighlighted;

		protected Renderer renderer;

		protected Collider collider;

		private bool m_Draw = true;

		public Material material
		{
			get
			{
				return (!renderer) ? null : renderer.material;
			}
		}

		public bool draw
		{
			get
			{
				return m_Draw;
			}
			set
			{
				if (m_Draw != value)
				{
					m_Draw = value;
					if ((bool)renderer)
					{
						renderer.enabled = m_Draw;
					}
					if ((bool)collider)
					{
						collider.enabled = m_Draw;
					}
				}
			}
		}

		protected Color colorNow
		{
			set
			{
				if ((bool)material)
				{
					material.color = value;
				}
			}
		}

		public bool isDrag { get; private set; }

		public CustomGuideObject guideObject { get; set; }

		protected Color ConvertColor(Color _color)
		{
			_color.r *= 0.75f;
			_color.g *= 0.75f;
			_color.b *= 0.75f;
			_color.a = 0.25f;
			return _color;
		}

		public void OnPointerEnter(PointerEventData eventData)
		{
			if (!GlobalMethod.IsCameraActionFlag(guideObject.ccv2))
			{
				colorNow = colorHighlighted;
			}
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			if (!isDrag)
			{
				colorNow = colorNormal;
			}
		}

		public virtual void OnBeginDrag(PointerEventData eventData)
		{
			if (!GlobalMethod.IsCameraActionFlag(guideObject.ccv2))
			{
				isDrag = true;
			}
		}

		public virtual void OnDrag(PointerEventData eventData)
		{
		}

		public virtual void OnEndDrag(PointerEventData eventData)
		{
			isDrag = false;
			colorNow = colorNormal;
		}

		private void OnDisable()
		{
			colorNow = colorNormal;
		}

		public virtual void Start()
		{
			renderer = base.gameObject.GetComponent<Renderer>();
			if (renderer == null)
			{
				renderer = base.gameObject.GetComponentInChildren<Renderer>();
			}
			collider = renderer.GetComponent<Collider>();
			colorNormal = ConvertColor(material.color);
			colorHighlighted = material.color;
			colorHighlighted.a = 0.75f;
			if ((bool)renderer)
			{
				renderer.enabled = m_Draw;
			}
			if ((bool)collider)
			{
				collider.enabled = m_Draw;
			}
			colorNow = colorNormal;
			isDrag = false;
		}
	}
}
