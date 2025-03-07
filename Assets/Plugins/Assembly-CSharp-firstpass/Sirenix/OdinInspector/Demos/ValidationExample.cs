using UnityEngine;

namespace Sirenix.OdinInspector.Demos
{
	public class ValidationExample : MonoBehaviour
	{
		[InfoBox("This is example demonstrates how to implement a custom drawer, that validates the property's value, and how to get Odin Scene Validator to pick up that validation warning or error.", InfoMessageType.Info, null)]
		[NotOne]
		public int NotOne;
	}
}
