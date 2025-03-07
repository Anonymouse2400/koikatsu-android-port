using UnityEngine;

namespace Sirenix.OdinInspector.Demos
{
	public class CombineGroupAttributeExample3 : MonoBehaviour
	{
		[Range(0f, 10f)]
		[LabelWidth(20f)]
		[HorizontalGroup("Split", 0.5f, 0, 0, 0)]
		[FoldoutGroup("Split/Left", 0)]
		[FoldoutGroup("Split/Right", 0)]
		[BoxGroup("Box", true, false, 0)]
		[BoxGroup("Split/Left/Box", true, false, 0)]
		[BoxGroup("Split/Right/Box", true, false, 0)]
		public float A;

		[FoldoutGroup("Split/Right", 0)]
		[FoldoutGroup("Split/Left", 0)]
		private void Button()
		{
		}
	}
}
