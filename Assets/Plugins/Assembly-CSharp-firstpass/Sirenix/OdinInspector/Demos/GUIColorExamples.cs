using UnityEngine;

namespace Sirenix.OdinInspector.Demos
{
	public class GUIColorExamples : MonoBehaviour
	{
		[Header("Test")]
		[GUIColor(0.3f, 0.8f, 0.8f, 1f)]
		public int ColoredInt1;

		[GUIColor(0.3f, 0.8f, 0.8f, 1f)]
		public int ColoredInt2;

		[ButtonGroup("_DefaultGroup", 0)]
		[GUIColor(0f, 1f, 0f, 1f)]
		private void Apply()
		{
		}

		[ButtonGroup("_DefaultGroup", 0)]
		[GUIColor(1f, 0.6f, 0.4f, 1f)]
		private void Cancel()
		{
		}
	}
}
