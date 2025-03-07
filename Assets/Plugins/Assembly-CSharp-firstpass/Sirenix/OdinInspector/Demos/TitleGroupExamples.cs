using UnityEngine;

namespace Sirenix.OdinInspector.Demos
{
	public class TitleGroupExamples : MonoBehaviour
	{
		[TitleGroup("Ints", null, TitleAlignments.Left, true, true, false, 0)]
		public int SomeInt1;

		[TitleGroup("$SomeString1", "Optional subtitle", TitleAlignments.Left, true, true, false, 0)]
		public string SomeString1;

		[TitleGroup("Vectors", "Optional subtitle", TitleAlignments.Centered, true, true, false, 0)]
		public Vector2 SomeVector1;

		[TitleGroup("Ints", "Optional subtitle", TitleAlignments.Split, true, true, false, 0)]
		public int SomeInt2;

		[TitleGroup("$SomeString1", "Optional subtitle", TitleAlignments.Left, true, true, false, 0)]
		public string SomeString2;

		[TitleGroup("Vectors", null, TitleAlignments.Left, true, true, false, 0)]
		public Vector2 SomeVector2 { get; set; }

		[TitleGroup("Ints/Buttons", null, TitleAlignments.Left, true, true, false, 0)]
		private void IntButton()
		{
		}

		[TitleGroup("$SomeString1/Buttons", null, TitleAlignments.Left, true, true, false, 0)]
		private void StringButton()
		{
		}

		[TitleGroup("Vectors", null, TitleAlignments.Left, true, true, false, 0)]
		private void VectorButton()
		{
		}
	}
}
