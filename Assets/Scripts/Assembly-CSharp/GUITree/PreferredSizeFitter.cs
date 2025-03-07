using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GUITree
{
	[ExecuteInEditMode]
	[RequireComponent(typeof(RectTransform))]
	[AddComponentMenu("GUITree/Preferred Size Fitter", 1001)]
	public class PreferredSizeFitter : UIBehaviour, ITreeLayoutElement, ILayoutSelfController, ILayoutElement, ILayoutController
	{
		[SerializeField]
		private float m_PreferredWidth = -1f;

		[SerializeField]
		private float m_PreferredHeight = -1f;

		[NonSerialized]
		private RectTransform m_Rect;

		private DrivenRectTransformTracker m_Tracker;

		public virtual float minWidth
		{
			get
			{
				return m_PreferredWidth;
			}
		}

		public virtual float minHeight
		{
			get
			{
				return m_PreferredHeight;
			}
		}

		public virtual float preferredWidth
		{
			get
			{
				return m_PreferredWidth;
			}
			set
			{
				if (SetPropertyUtility.SetStruct(ref m_PreferredWidth, value))
				{
					SetDirty();
				}
			}
		}

		public virtual float preferredHeight
		{
			get
			{
				return m_PreferredHeight;
			}
			set
			{
				if (SetPropertyUtility.SetStruct(ref m_PreferredHeight, value))
				{
					SetDirty();
				}
			}
		}

		public virtual float flexibleWidth
		{
			get
			{
				return m_PreferredWidth;
			}
		}

		public virtual float flexibleHeight
		{
			get
			{
				return m_PreferredHeight;
			}
		}

		public virtual int layoutPriority
		{
			get
			{
				return int.MaxValue;
			}
		}

		private RectTransform rectTransform
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

		protected PreferredSizeFitter()
		{
		}

		public virtual void CalculateLayoutInputHorizontal()
		{
		}

		public virtual void CalculateLayoutInputVertical()
		{
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

		protected override void OnRectTransformDimensionsChange()
		{
			SetDirty();
		}

		private void HandleSelfFittingAlongAxis(int axis)
		{
			m_Tracker.Add(this, rectTransform, (axis != 0) ? DrivenTransformProperties.SizeDeltaY : DrivenTransformProperties.SizeDeltaX);
			rectTransform.SetSizeWithCurrentAnchors((RectTransform.Axis)axis, LayoutUtility.GetPreferredSize(this, axis));
		}

		public virtual void SetLayoutHorizontal()
		{
			m_Tracker.Clear();
			HandleSelfFittingAlongAxis(0);
		}

		public virtual void SetLayoutVertical()
		{
			HandleSelfFittingAlongAxis(1);
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
