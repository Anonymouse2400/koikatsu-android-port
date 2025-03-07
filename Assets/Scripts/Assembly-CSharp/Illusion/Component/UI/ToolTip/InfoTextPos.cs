using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Illusion.Component.UI.ToolTip
{
	public class InfoTextPos : InfoPos
	{
		[Tooltip("カーソルが当たった場所に表示")]
		[SerializeField]
		private bool isTargetSet;

		[Tooltip("折り返したいテキストの幅")]
		[SerializeField]
		private float textReturnWidth = -1f;

		private TextWindow _textWindow;

		private TextWindow textWindow
		{
			get
			{
				return info.GetComponentCache(ref _textWindow);
			}
		}

		private static List<RaycastResult> HitList
		{
			get
			{
				List<RaycastResult> list = new List<RaycastResult>();
				EventSystem.current.RaycastAll(new PointerEventData(EventSystem.current)
				{
					position = Input.mousePosition
				}, list);
				return list;
			}
		}

		protected override void Update()
		{
			base.target = null;
			List<RaycastResult> hitList = HitList;
			string text = string.Empty;
			float? width = null;
			if (hitList.Count > 0)
			{
				foreach (RaycastResult item in hitList)
				{
					InfoText component = item.gameObject.GetComponent<InfoText>();
					if (!(component != null))
					{
						continue;
					}
					string data = component.data;
					if (data != null)
					{
						if (isTargetSet)
						{
							base.target = item.gameObject.transform;
						}
						text = data;
						if (textReturnWidth > 0f)
						{
							width = textReturnWidth;
						}
						break;
					}
				}
			}
			textWindow.SetText(text, width);
			base.Update();
		}
	}
}
