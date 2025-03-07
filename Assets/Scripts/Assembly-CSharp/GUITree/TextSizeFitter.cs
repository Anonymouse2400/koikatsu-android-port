using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace GUITree
{
	[ExecuteInEditMode]
	[AddComponentMenu("GUITree/Text Size Fitter", 1002)]
	[RequireComponent(typeof(RectTransform))]
	public class TextSizeFitter : UIBehaviour, ITreeLayoutElement, ILayoutGroup, ILayoutElement, ILayoutController
	{
		[SerializeField]
		protected RectOffset m_Padding = new RectOffset();

		[FormerlySerializedAs("m_Alignment")]
		[SerializeField]
		protected TextAnchor m_ChildAlignment;

		[NonSerialized]
		private RectTransform m_Rect;

		protected DrivenRectTransformTracker m_Tracker;

		private Vector2 m_TotalMinSize = Vector2.zero;

		private Vector2 m_TotalPreferredSize = Vector2.zero;

		private Vector2 m_TotalFlexibleSize = Vector2.zero;

		private Text m_Text;

		private RectTransform m_RectText;

		private ILayoutElement m_ElementText;

		private ContentSizeFitter.FitMode m_FitModeHorizontal;

		private ContentSizeFitter.FitMode m_FitModeVertical;

		[SerializeField]
		protected bool m_ChildForceExpandWidth = true;

		[SerializeField]
		protected bool m_ChildForceExpandHeight = true;

		public RectOffset padding
		{
			get
			{
				return m_Padding;
			}
			set
			{
				SetProperty(ref m_Padding, value);
			}
		}

		public TextAnchor childAlignment
		{
			get
			{
				return m_ChildAlignment;
			}
			set
			{
				SetProperty(ref m_ChildAlignment, value);
			}
		}

		protected RectTransform rectTransform
		{
			get
			{
				if (m_Rect == null)
				{
					m_Rect = GetComponent<RectTransform>();
				}
				return m_Rect;
			}
		}

		protected Text text
		{
			get
			{
				if (m_Text == null)
				{
					m_Text = GetComponentInChildren<Text>();
				}
				return m_Text;
			}
		}

		protected RectTransform rectText
		{
			get
			{
				if (m_RectText == null && text != null)
				{
					m_RectText = text.rectTransform;
				}
				return m_RectText;
			}
		}

		protected ILayoutElement elementText
		{
			get
			{
				if (m_ElementText == null && text != null)
				{
					m_ElementText = text.GetComponent<ILayoutElement>();
				}
				return m_ElementText;
			}
		}

		private bool isContentSizeFitter { get; set; }

		public virtual float minWidth
		{
			get
			{
				return GetTotalMinSize(0);
			}
		}

		public virtual float preferredWidth
		{
			get
			{
				return GetTotalPreferredSize(0);
			}
		}

		public virtual float flexibleWidth
		{
			get
			{
				return GetTotalFlexibleSize(0);
			}
		}

		public virtual float minHeight
		{
			get
			{
				return GetTotalMinSize(1);
			}
		}

		public virtual float preferredHeight
		{
			get
			{
				return GetTotalPreferredSize(1);
			}
		}

		public virtual float flexibleHeight
		{
			get
			{
				return GetTotalFlexibleSize(1);
			}
		}

		public virtual int layoutPriority
		{
			get
			{
				return int.MaxValue;
			}
		}

		public bool childForceExpandWidth
		{
			get
			{
				return m_ChildForceExpandWidth;
			}
			set
			{
				SetProperty(ref m_ChildForceExpandWidth, value);
			}
		}

		public bool childForceExpandHeight
		{
			get
			{
				return m_ChildForceExpandHeight;
			}
			set
			{
				SetProperty(ref m_ChildForceExpandHeight, value);
			}
		}

		protected TextSizeFitter()
		{
			if (m_Padding == null)
			{
				m_Padding = new RectOffset();
			}
		}

		public void CalculateLayoutInputHorizontal()
		{
			m_Tracker.Clear();
			ContentSizeFitter component = rectTransform.GetComponent<ContentSizeFitter>();
			isContentSizeFitter = component != null;
			if (isContentSizeFitter)
			{
				m_FitModeHorizontal = component.horizontalFit;
				m_FitModeVertical = component.verticalFit;
			}
			CalcAlongAxis(0);
		}

		public void CalculateLayoutInputVertical()
		{
			CalcAlongAxis(1);
		}

		public void SetLayoutHorizontal()
		{
			SetChildrenAlongAxis(0);
		}

		public void SetLayoutVertical()
		{
			SetChildrenAlongAxis(1);
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			SetDirty();
		}

		protected override void OnDisable()
		{
			m_Tracker.Clear();
			if (rectTransform != null)
			{
				LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
			}
			base.OnDisable();
		}

		protected override void OnDidApplyAnimationProperties()
		{
			SetDirty();
		}

		protected void CalcAlongAxis(int axis)
		{
			float num = ((axis != 0) ? padding.vertical : padding.horizontal);
			float num2 = num;
			float num3 = num;
			float num4 = 0f;
			bool flag = axis == 1;
			float minSize = LayoutUtility.GetMinSize(elementText, axis);
			float preferredSize = LayoutUtility.GetPreferredSize(elementText, axis);
			float num5 = LayoutUtility.GetFlexibleSize(elementText, axis);
			if ((axis != 0) ? childForceExpandHeight : childForceExpandWidth)
			{
				num5 = Mathf.Max(num5, 1f);
			}
			if (flag)
			{
				num2 = Mathf.Max(minSize + num, num2);
				num3 = Mathf.Max(preferredSize + num, num3);
				num4 = Mathf.Max(num5, num4);
			}
			else
			{
				num2 += minSize;
				num3 += preferredSize;
				num4 += num5;
			}
			num3 = Mathf.Max(num2, num3);
			SetLayoutInputForAxis(num2, num3, num4, axis);
		}

		protected void SetChildrenAlongAxis(int axis)
		{
			float num = rectTransform.rect.size[axis];
			int num2;
			switch (axis)
			{
			case 1:
			{
				float value = num - (float)((axis != 0) ? padding.vertical : padding.horizontal);
				float minSize = LayoutUtility.GetMinSize(elementText, axis);
				float preferredSize = LayoutUtility.GetPreferredSize(elementText, axis);
				float num3 = LayoutUtility.GetFlexibleSize(elementText, axis);
				if ((axis != 0) ? childForceExpandHeight : childForceExpandWidth)
				{
					num3 = Mathf.Max(num3, 1f);
				}
				float num4 = Mathf.Clamp(value, minSize, (!(num3 > 0f)) ? preferredSize : num);
				float startOffset = GetStartOffset(axis, num4);
				SetChildAlongAxis(rectText, axis, startOffset, num4);
				return;
			}
			case 0:
				num2 = padding.left;
				break;
			default:
				num2 = padding.top;
				break;
			}
			float num5 = num2;
			if (GetTotalFlexibleSize(axis) == 0f && GetTotalPreferredSize(axis) < num)
			{
				num5 = GetStartOffset(axis, GetTotalPreferredSize(axis) - (float)((axis != 0) ? padding.vertical : padding.horizontal));
			}
			float t = 0f;
			if (GetTotalMinSize(axis) != GetTotalPreferredSize(axis))
			{
				t = Mathf.Clamp01((num - GetTotalMinSize(axis)) / (GetTotalPreferredSize(axis) - GetTotalMinSize(axis)));
			}
			float num6 = 0f;
			if (num > GetTotalPreferredSize(axis) && GetTotalFlexibleSize(axis) > 0f)
			{
				num6 = (num - GetTotalPreferredSize(axis)) / GetTotalFlexibleSize(axis);
			}
			float minSize2 = LayoutUtility.GetMinSize(elementText, axis);
			float preferredSize2 = LayoutUtility.GetPreferredSize(elementText, axis);
			float num7 = LayoutUtility.GetFlexibleSize(elementText, axis);
			if ((axis != 0) ? childForceExpandHeight : childForceExpandWidth)
			{
				num7 = Mathf.Max(num7, 1f);
			}
			float num8 = Mathf.Lerp(minSize2, preferredSize2, t);
			num8 += num7 * num6;
			SetChildAlongAxis(rectText, axis, num5, num8);
			num5 += num8;
		}

		protected float GetTotalMinSize(int axis)
		{
			return m_TotalMinSize[axis];
		}

		protected float GetTotalPreferredSize(int axis)
		{
			return (isContentSizeFitter && !(isContentSizeFitter & (((axis != 0) ? m_FitModeVertical : m_FitModeHorizontal) == ContentSizeFitter.FitMode.PreferredSize))) ? rectTransform.sizeDelta[axis] : m_TotalPreferredSize[axis];
		}

		protected float GetTotalFlexibleSize(int axis)
		{
			return m_TotalFlexibleSize[axis];
		}

		protected float GetStartOffset(int axis, float requiredSpaceWithoutPadding)
		{
			float num = requiredSpaceWithoutPadding + (float)((axis != 0) ? padding.vertical : padding.horizontal);
			float num2 = rectTransform.rect.size[axis];
			float num3 = num2 - num;
			float num4 = 0f;
			num4 = ((axis != 0) ? ((float)((int)childAlignment / 3) * 0.5f) : ((float)((int)childAlignment % 3) * 0.5f));
			return (float)((axis != 0) ? padding.top : padding.left) + num3 * num4;
		}

		protected void SetLayoutInputForAxis(float totalMin, float totalPreferred, float totalFlexible, int axis)
		{
			m_TotalMinSize[axis] = totalMin;
			m_TotalPreferredSize[axis] = totalPreferred;
			m_TotalFlexibleSize[axis] = totalFlexible;
		}

		protected void SetChildAlongAxis(RectTransform rect, int axis, float pos, float size)
		{
			if (!(rect == null))
			{
				m_Tracker.Add(this, rect, DrivenTransformProperties.Anchors | DrivenTransformProperties.AnchoredPosition | DrivenTransformProperties.SizeDelta);
				rect.SetInsetAndSizeFromParentEdge((axis != 0) ? RectTransform.Edge.Top : RectTransform.Edge.Left, pos, size);
			}
		}

		protected override void OnRectTransformDimensionsChange()
		{
			base.OnRectTransformDimensionsChange();
		}

		protected virtual void OnTransformChildrenChanged()
		{
			SetDirty();
		}

		protected void SetProperty<T>(ref T currentValue, T newValue)
		{
			if ((currentValue != null || newValue != null) && (currentValue == null || !currentValue.Equals(newValue)))
			{
				currentValue = newValue;
				SetDirty();
			}
		}

		protected void SetDirty()
		{
			if (IsActive() && rectTransform != null)
			{
				LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
			}
		}
	}
}
