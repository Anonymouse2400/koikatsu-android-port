using UnityEngine;

namespace Sirenix.OdinInspector.Demos
{
	public class ColorPaletteExamples : MonoBehaviour
	{
		[ColorPalette]
		public Color ColorOptions;

		[ColorPalette("Underwater")]
		public Color UnderwaterColor;

		[ColorPalette("Fall")]
		[HideLabel]
		public Color WideColorPalette;

		[ColorPalette("My Palette")]
		public Color MyColor;

		[ColorPalette("Clovers")]
		public Color[] ColorArray;
	}
}
