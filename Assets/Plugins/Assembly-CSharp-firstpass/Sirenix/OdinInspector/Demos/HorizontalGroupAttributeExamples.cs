using UnityEngine;

namespace Sirenix.OdinInspector.Demos
{
	public class HorizontalGroupAttributeExamples : MonoBehaviour
	{
		[HorizontalGroup(0f, 0, 0, 0)]
		public int A;

		[HideLabel]
		[LabelWidth(150f)]
		[HorizontalGroup(150f, 0, 0, 0)]
		public LayerMask B;

		[HorizontalGroup("Group 1", 0f, 0, 0, 0)]
		[LabelWidth(15f)]
		public int C;

		[HorizontalGroup("Group 1", 0f, 0, 0, 0)]
		[LabelWidth(15f)]
		public int D;

		[HorizontalGroup("Group 1", 0f, 0, 0, 0)]
		[LabelWidth(15f)]
		public int E;

		[HorizontalGroup("Split", 0.5f, 0, 0, 0, PaddingRight = 5f)]
		[BoxGroup("Split/Left", true, false, 0)]
		[LabelWidth(15f)]
		public int L;

		[HorizontalGroup("Split", 0.5f, 0, 0, 0, PaddingLeft = 5f)]
		[BoxGroup("Split/Right", true, false, 0)]
		[LabelWidth(15f)]
		public int M;

		[BoxGroup("Split/Left", true, false, 0)]
		[LabelWidth(15f)]
		public int N;

		[BoxGroup("Split/Right", true, false, 0)]
		[LabelWidth(15f)]
		public int O;

		[HorizontalGroup("MyButton", 0f, 0, 0, 0, MarginLeft = 0.25f, MarginRight = 0.25f)]
		public void SomeButton()
		{
		}
	}
}
