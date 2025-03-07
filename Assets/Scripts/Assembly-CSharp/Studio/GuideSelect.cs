using UnityEngine;
using UnityEngine.EventSystems;

namespace Studio
{
	public class GuideSelect : GuideBase, IPointerClickHandler, IEventSystemHandler
	{
		public Color color
		{
			set
			{
				colorNormal = ConvertColor(value);
				colorHighlighted = new Color(value.r, value.g, value.b, 1f);
				base.colorNow = colorNormal;
			}
		}

		public TreeNodeObject treeNodeObject { get; set; }

		public void OnPointerClick(PointerEventData _eventData)
		{
			if (treeNodeObject != null)
			{
				treeNodeObject.Select();
			}
			else if (Singleton<GuideObjectManager>.IsInstance())
			{
				Singleton<GuideObjectManager>.Instance.selectObject = base.guideObject;
			}
		}

		private void Awake()
		{
			base.Start();
			treeNodeObject = null;
		}

		public override void Start()
		{
			base.colorNow = colorNormal;
		}
	}
}
