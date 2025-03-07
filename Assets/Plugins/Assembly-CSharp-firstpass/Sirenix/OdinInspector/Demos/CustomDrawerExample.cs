using UnityEngine;

namespace Sirenix.OdinInspector.Demos
{
	public class CustomDrawerExample : MonoBehaviour
	{
		[InfoBox("This example demonstrates how a custom drawer can be implemented for a custom struct or class.", InfoMessageType.Info, null)]
		public MyStruct MyStruct;
	}
}
