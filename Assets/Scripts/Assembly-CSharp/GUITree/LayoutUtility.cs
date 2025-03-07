using UnityEngine;
using UnityEngine.UI;

namespace GUITree
{
	public static class LayoutUtility
	{
		public static float GetMinSize(ILayoutElement _element, int _axis)
		{
			return (_axis != 0) ? GetMinHeight(_element) : GetMinWidth(_element);
		}

		public static float GetPreferredSize(ILayoutElement _element, int _axis)
		{
			return (_axis != 0) ? GetPreferredHeight(_element) : GetPreferredWidth(_element);
		}

		public static float GetFlexibleSize(ILayoutElement _element, int _axis)
		{
			return (_axis != 0) ? GetFlexibleHeight(_element) : GetFlexibleWidth(_element);
		}

		public static float GetMinWidth(ILayoutElement _element)
		{
			return (_element != null) ? _element.minWidth : 0f;
		}

		public static float GetPreferredWidth(ILayoutElement _element)
		{
			return (_element != null) ? Mathf.Max(Mathf.Max(_element.minWidth, _element.preferredWidth), 0f) : 0f;
		}

		public static float GetFlexibleWidth(ILayoutElement _element)
		{
			return (_element != null) ? _element.flexibleWidth : 0f;
		}

		public static float GetMinHeight(ILayoutElement _element)
		{
			return (_element != null) ? _element.minHeight : 0f;
		}

		public static float GetPreferredHeight(ILayoutElement _element)
		{
			return (_element != null) ? Mathf.Max(Mathf.Max(_element.minHeight, _element.preferredHeight), 0f) : 0f;
		}

		public static float GetFlexibleHeight(ILayoutElement _element)
		{
			return (_element != null) ? _element.flexibleHeight : 0f;
		}
	}
}
