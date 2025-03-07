using System;
using UnityEngine;

namespace Sirenix.OdinInspector.Demos
{
	public class BoxGroupExamples : MonoBehaviour
	{
		[Serializable]
		public struct SomeStruct
		{
			public int One;

			public int Two;

			public int Three;
		}

		[BoxGroup("Centered Title", true, true, 0)]
		public int A;

		[BoxGroup("Centered Title", true, true, 0)]
		public int B;

		[BoxGroup("Centered Title", true, true, 0)]
		public int C;

		[BoxGroup("Left Oriented Title", true, false, 0)]
		public int D;

		[BoxGroup("Left Oriented Title", true, false, 0)]
		public int E;

		[BoxGroup("$DynamicTitle1", true, false, 0)]
		[LabelText("Dynamic Title")]
		public string DynamicTitle1 = "Dynamic box title";

		[BoxGroup("$DynamicTitle1", true, false, 0)]
		public int F;

		[BoxGroup("$DynamicTitle2", true, false, 0)]
		public int G;

		[BoxGroup("$DynamicTitle2", true, false, 0)]
		public int H;

		[InfoBox("You can also hide the label of a box group.", InfoMessageType.Info, null)]
		[BoxGroup("NoTitle", false, false, 0)]
		public int I;

		[BoxGroup("NoTitle", true, false, 0)]
		public int J;

		[BoxGroup("NoTitle", true, false, 0)]
		public int K;

		[BoxGroup("Boxed Struct", true, false, 0)]
		[HideLabel]
		public SomeStruct BoxedStruct;

		public SomeStruct DefaultStruct;
	}
}
