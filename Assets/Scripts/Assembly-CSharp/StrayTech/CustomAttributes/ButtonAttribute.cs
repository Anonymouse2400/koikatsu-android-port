using UnityEngine;

namespace StrayTech.CustomAttributes
{
	public class ButtonAttribute : PropertyAttribute
	{
		public readonly string ButtonLabel = string.Empty;

		public readonly string ButtonText = string.Empty;

		public ButtonAttribute(string buttonText, string buttonLabel)
		{
			ButtonText = buttonText;
			ButtonLabel = buttonLabel;
		}

		public ButtonAttribute(string buttonText)
		{
			ButtonText = buttonText;
		}

		public ButtonAttribute()
		{
		}
	}
}
