using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GUITree
{
	[RequireComponent(typeof(RectTransform))]
	[AddComponentMenu("GUITree/Tree Node", 1000)]
	public class TreeNode : UIBehaviour, ITreeLayoutElement, ILayoutSelfController, ILayoutElement, ILayoutController
	{
		private float m_PreferredWidth = -1f;

		private float m_PreferredHeight = -1f;

		[NonSerialized]
		private RectTransform m_Rect;

		private DrivenRectTransformTracker m_Tracker;

		[SerializeField]
		private int m_Indent;

		[SerializeField]
		private float m_IndentSize = 32f;

		public virtual float minWidth
		{
			get
			{
				return preferredWidth;
			}
		}

		public virtual float preferredWidth
		{
			get
			{
				return m_PreferredWidth + (float)indent * indentSize;
			}
		}

		public virtual float flexibleWidth
		{
			get
			{
				return preferredWidth;
			}
		}

		public virtual float minHeight
		{
			get
			{
				return preferredHeight;
			}
		}

		public virtual float preferredHeight
		{
			get
			{
				return m_PreferredHeight;
			}
		}

		public virtual float flexibleHeight
		{
			get
			{
				return preferredHeight;
			}
		}

		public int layoutPriority
		{
			get
			{
				return int.MaxValue;
			}
		}

		public RectTransform rectTransform
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

		public int indent
		{
			get
			{
				return m_Indent;
			}
			set
			{
				if (SetPropertyUtility.SetStruct(ref m_Indent, value))
				{
					SetDirty();
				}
			}
		}

		public float indentSize
		{
			get
			{
				return m_IndentSize;
			}
			set
			{
				if (SetPropertyUtility.SetStruct(ref m_IndentSize, value))
				{
					SetDirty();
				}
			}
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			SetDirty();
		}

		protected override void OnDisable()
		{
			m_Tracker.Clear();
			if ((bool)rectTransform)
			{
				LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
			}
			base.OnDisable();
		}

		public void CalculateLayoutInputHorizontal()
		{
			int childCount = rectTransform.childCount;
			float num = 0f;
			for (int i = 0; i < childCount; i++)
			{
				ITreeLayoutElement component = rectTransform.GetChild(i).GetComponent<ITreeLayoutElement>();
				if (component != null)
				{
					num += LayoutUtility.GetPreferredSize(component, 0);
				}
			}
			m_PreferredWidth = num;
		}

		public void CalculateLayoutInputVertical()
		{
			int childCount = rectTransform.childCount;
			float num = 0f;
			for (int i = 0; i < childCount; i++)
			{
				ITreeLayoutElement component = rectTransform.GetChild(i).GetComponent<ITreeLayoutElement>();
				if (component != null)
				{
					float preferredSize = LayoutUtility.GetPreferredSize(component, 1);
					if (num < preferredSize)
					{
						num = preferredSize;
					}
				}
			}
			m_PreferredHeight = num;
		}

		public void SetLayoutHorizontal()
		{
			m_Tracker.Clear();
			m_Tracker.Add(this, rectTransform, DrivenTransformProperties.AnchoredPositionX | DrivenTransformProperties.SizeDeltaX);
			rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, (float)indent * indentSize, LayoutUtility.GetPreferredSize(this, 0));
		}

		public void SetLayoutVertical()
		{
			m_Tracker.Add(this, rectTransform, DrivenTransformProperties.SizeDeltaY);
			rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, LayoutUtility.GetPreferredSize(this, 1));
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
