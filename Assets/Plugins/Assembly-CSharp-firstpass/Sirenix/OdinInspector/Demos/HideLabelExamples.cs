using UnityEngine;

namespace Sirenix.OdinInspector.Demos
{
	public class HideLabelExamples : MonoBehaviour
	{
		[Title("Wide Colors", null, TitleAlignments.Left, true, true)]
		[HideLabel]
		[ColorPalette("Fall")]
		public Color WideColor1;

		[HideLabel]
		[ColorPalette("Fall")]
		public Color WideColor2;

		[Title("Wide Vector", null, TitleAlignments.Left, true, true)]
		[HideLabel]
		public Vector3 WideVector1;

		[HideLabel]
		public Vector4 WideVector2;

		[Title("Wide String", null, TitleAlignments.Left, true, true)]
		[HideLabel]
		public string WideString;

		[Title("Wide Multiline Text Field", null, TitleAlignments.Left, true, true)]
		[HideLabel]
		[MultiLineProperty(3)]
		public string WideMultilineTextField = string.Empty;
	}
}
