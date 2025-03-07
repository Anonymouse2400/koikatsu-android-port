using System;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Studio
{
	public class GuideBase : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IEventSystemHandler
	{
		[SerializeField]
		protected Color colorNormal;

		[SerializeField]
		protected Color colorHighlighted;

		protected Renderer renderer;

		protected Collider collider;

		private BoolReactiveProperty _draw = new BoolReactiveProperty(true);

		public Action<Transform> pointerEnterAction;

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
				return _draw.Value;
			}
			set
			{
				_draw.Value = value;
			}
		}

		protected Color colorNow
		{
			set
			{
				if ((bool)material)
				{
					material.color = value;
					if (material.HasProperty("_EmissionColor"))
					{
						material.SetColor("_EmissionColor", value);
					}
				}
			}
		}

		public bool isDrag { get; private set; }

		public GuideObject guideObject { get; set; }

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
			if (!Singleton<GuideObjectManager>.Instance.isOperationTarget)
			{
				colorNow = colorHighlighted;
				if (pointerEnterAction != null)
				{
					pointerEnterAction(base.transform);
				}
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
			isDrag = true;
			Singleton<GuideObjectManager>.Instance.operationTarget = guideObject;
		}

		public virtual void OnDrag(PointerEventData eventData)
		{
		}

		public virtual void OnEndDrag(PointerEventData eventData)
		{
			isDrag = false;
			colorNow = colorNormal;
			Singleton<GuideObjectManager>.Instance.operationTarget = null;
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
			_draw.Subscribe(delegate(bool _b)
			{
				if ((bool)renderer)
				{
					renderer.enabled = _b;
				}
				if ((bool)collider)
				{
					collider.enabled = _b;
				}
			});
			if ((bool)renderer)
			{
				renderer.enabled = draw;
			}
			if ((bool)collider)
			{
				collider.enabled = draw;
			}
			colorNow = colorNormal;
			isDrag = false;
		}
	}
}
