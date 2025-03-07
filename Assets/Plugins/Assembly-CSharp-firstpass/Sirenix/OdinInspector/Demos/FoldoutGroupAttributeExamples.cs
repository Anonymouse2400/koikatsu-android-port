using UnityEngine;

namespace Sirenix.OdinInspector.Demos
{
	public class FoldoutGroupAttributeExamples : MonoBehaviour
	{
		[FoldoutGroup("Group 1", 0)]
		public int A;

		[FoldoutGroup("Group 1", 0)]
		public int B;

		[FoldoutGroup("Group 1", 0)]
		public int C;

		[FoldoutGroup("Collapsed group", false, 0)]
		public int D;

		[FoldoutGroup("Collapsed group", 0)]
		public int E;

		[FoldoutGroup("$GroupTitle", true, 0)]
		public int One;

		[FoldoutGroup("$GroupTitle", 0)]
		public int Two;

		public string GroupTitle = "Dynamic group title";
	}
}
