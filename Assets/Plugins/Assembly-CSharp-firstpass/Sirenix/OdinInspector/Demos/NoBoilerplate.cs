using System.Collections.Generic;
using UnityEngine;

namespace Sirenix.OdinInspector.Demos
{
	public class NoBoilerplate : MonoBehaviour
	{
		[TabGroup("Tab", "Tab 1", false, 0)]
		[AssetList]
		[InlineEditor(InlineEditorModes.SmallPreview)]
		public GameObject GameObject;

		[TabGroup("Tab", "Tab 1", false, 0)]
		private Quaternion Quaternion;

		[TabGroup("Tab", "Tab 2", false, 0)]
		private Vector3 Vector3;

		[FoldoutGroup("Tab/Tab 2/Vector4s", 0)]
		private Vector4 A;

		[FoldoutGroup("Tab/Tab 2/Vector4s", 0)]
		private Vector4 B;

		[FoldoutGroup("Tab/Tab 2/Vector4s", 0)]
		private Vector4 C;

		[Multiline]
		[HideLabel]
		[Title("Enter text:", null, TitleAlignments.Left, true, false)]
		public string MyTextArea;

		[ColorPalette("Fall")]
		[HorizontalGroup(0.5f, 0, 3, 0)]
		public List<Color> ColorArray1;

		[ColorPalette("Fall")]
		[HorizontalGroup(0f, 0, 0, 0)]
		public List<Color> ColorArray2;

		[Range(0f, 1f)]
		public float[] FloatRange;

		[ShowInInspector]
		[DisplayAsString]
		public string Property
		{
			get
			{
				return "Support!";
			}
		}

		[Button(ButtonSizes.Small)]
		private void SayHi()
		{
			Debug.Log("Yo!");
		}
	}
}
