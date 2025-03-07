using UnityEngine;

namespace Sirenix.OdinInspector.Demos
{
	public class VerticalGroupExamples : MonoBehaviour
	{
		[HorizontalGroup("Split", 0f, 0, 0, 0)]
		[VerticalGroup("Split/Left", 0)]
		[LabelWidth(100f)]
		public Vector3 Vector;

		[VerticalGroup("Split/Left", 0)]
		[LabelWidth(100f)]
		public GameObject First;

		[VerticalGroup("Split/Left", 0)]
		[LabelWidth(100f)]
		public GameObject Second;

		[HideLabel]
		[VerticalGroup("Split/Right", 0, PaddingTop = 18f)]
		public int A;

		[HideLabel]
		[VerticalGroup("Split/Right", 0)]
		public int B;
	}
}
