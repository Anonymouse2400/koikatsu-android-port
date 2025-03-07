using TMPro;
using UnityEngine;

namespace Localize.Translate
{
	public class UITextMeshSetting : UITextSetting
	{
		[SerializeField]
		private bool _useAnchor;

		[SerializeField]
		private TextAlignmentOptions _anchor;

		public override bool Set(TMP_Text text)
		{
			if (text == null)
			{
				return false;
			}
			bool? flag = null;
			if (base.fontSize > 0f)
			{
				text.fontSize = base.fontSize;
				flag = false;
			}
			else
			{
				if (base.fontSizeMin > 0f)
				{
					text.fontSizeMin = base.fontSizeMin;
					flag = true;
				}
				if (base.fontSizeMax > 0f)
				{
					text.fontSizeMax = base.fontSizeMax;
					flag = true;
				}
			}
			if (flag.HasValue)
			{
				text.enableAutoSizing = flag.Value;
			}
			if (_useAnchor)
			{
				text.alignment = _anchor;
			}
			return true;
		}
	}
}
