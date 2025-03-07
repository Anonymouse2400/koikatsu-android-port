using UnityEngine;

namespace Sirenix.OdinInspector.Demos
{
	public class ToggleLeftExamples : MonoBehaviour
	{
		[InfoBox("Draws the toggle button before the label for a bool property.", InfoMessageType.Info, null)]
		[ToggleLeft]
		public bool LeftToggled;

		[EnableIf("LeftToggled")]
		public int A;

		[EnableIf("LeftToggled")]
		public bool B;

		[EnableIf("LeftToggled")]
		public bool C;
	}
}
