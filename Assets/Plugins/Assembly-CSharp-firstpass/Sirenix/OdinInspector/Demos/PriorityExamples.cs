using UnityEngine;

namespace Sirenix.OdinInspector.Demos
{
	public class PriorityExamples : MonoBehaviour
	{
		[InfoBox("In this example, we have three different drawers, with different priorities, all drawing the same value.\n\nThe purpose is to demonstrate the drawer chain, and the general purpose of each drawer priority.", InfoMessageType.Info, null)]
		[ShowDrawerChain]
		public MyClass MyClass;
	}
}
