using UnityEngine;

namespace Sirenix.OdinInspector.Demos
{
	public class CombineGroupAttributeExample2 : MonoBehaviour
	{
		[TabGroup("MyTabGroup", "Tab1", false, 0)]
		public int[] A;

		[TabGroup("MyTabGroup", "Tab2", false, 0)]
		public int C;

		[TabGroup("MyTabGroup", "Tab1", false, 0)]
		public int[] B;

		[BoxGroup("MyTabGroup/Tab2/Box", true, false, 0)]
		public int D;

		[BoxGroup("MyTabGroup/Tab2/Box", true, false, 0)]
		public int E;

		[BoxGroup("MyTabGroup/Tab2/Box", true, false, 0)]
		public int F;

		[HorizontalGroup("MyTabGroup/Tab1/Split", 0.5f, 0, 0, 0)]
		public int[] G;

		[HorizontalGroup("MyTabGroup/Tab1/Split", 0f, 0, 0, 0)]
		public int[] H;

		[TabGroup("MyTabGroup", "Tab2", false, 0)]
		public int I;
	}
}
