using UnityEngine;

namespace Sirenix.OdinInspector.Demos
{
	public class CombineGroupAttributeExample1 : MonoBehaviour
	{
		[HorizontalGroup("Split", 0.4f, 0, 0, 0)]
		[BoxGroup("Split/Left", true, false, 0)]
		public int[] A;

		[BoxGroup("Split/Left", true, false, 0)]
		public int[] C;

		[BoxGroup("Split/Center", true, false, 0)]
		public int[] B;

		[BoxGroup("Split/Center", true, false, 0)]
		public int[] D;

		[HorizontalGroup("Split", 0.4f, 0, 0, 0)]
		[BoxGroup("Split/Right", true, false, 0)]
		public int[] E;

		[BoxGroup("Split/Right", true, false, 0)]
		public int[] F;
	}
}
