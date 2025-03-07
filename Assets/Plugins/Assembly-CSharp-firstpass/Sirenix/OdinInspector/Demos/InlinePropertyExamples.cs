using UnityEngine;

namespace Sirenix.OdinInspector.Demos
{
	public class InlinePropertyExamples : MonoBehaviour
	{
		public Vector3 Vector3;

		public Vector3Int Vector3Int;

		[InlineProperty(LabelWidth = 15)]
		public Vector2Int Vector2Int;
	}
}
