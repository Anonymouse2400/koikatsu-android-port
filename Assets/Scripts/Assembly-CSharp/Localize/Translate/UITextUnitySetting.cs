using UnityEngine;
using UnityEngine.UI;

namespace Localize.Translate
{
	public class UITextUnitySetting : UITextSetting
	{
		[SerializeField]
		private bool _useAnchor;

		[SerializeField]
		private TextAnchor _anchor;

		public override bool Set(Text text)
		{
			if (text == null)
			{
				return false;
			}
			bool? flag = null;
			if (base.fontSize > 0f)
			{
				text.fontSize = (int)base.fontSize;
				flag = false;
			}
			else
			{
				if (base.fontSizeMin > 0f)
				{
					text.resizeTextMinSize = (int)base.fontSizeMin;
					flag = true;
				}
				if (base.fontSizeMax > 0f)
				{
					text.resizeTextMaxSize = (int)base.fontSizeMax;
					flag = true;
				}
			}
			if (flag.HasValue)
			{
				text.resizeTextForBestFit = flag.Value;
			}
			if (_useAnchor)
			{
				text.alignment = _anchor;
			}
			return true;
		}
	}
}
