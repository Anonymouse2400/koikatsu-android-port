using UnityEngine;

namespace Sirenix.OdinInspector.Demos
{
	public class CustomContextMenuExamples : MonoBehaviour
	{
		[InfoBox("A custom context menu is added on this property. Right click the property to view the custom context menu.", InfoMessageType.Info, null)]
		[CustomContextMenu("Say Hello/Twice", "SayHello")]
		public int MyProperty;

		private void SayHello()
		{
			Debug.Log("Hello");
		}
	}
}
