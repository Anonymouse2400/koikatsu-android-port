using UnityEngine;

namespace Sirenix.OdinInspector.Demos
{
	public class DisplayAsStringExamples : MonoBehaviour
	{
		[InfoBox("Instead of disabling values in the inspector in order to show some information or debug a value. You can use DisplayAsString to show the value as text, instead of showing it in a disabled drawer", InfoMessageType.Info, null)]
		[DisplayAsString]
		public Color SomeColor;

		[BoxGroup("SomeBox", true, false, 0)]
		[DisplayAsString]
		[HideLabel]
		public string SomeText = "Lorem Ipsum";

		[InfoBox("The DisplayAsString attribute can also be configured to enable or disable overflowing to multiple lines.", InfoMessageType.Info, null)]
		[DisplayAsString]
		[HideLabel]
		public string Overflow = "A very long string that has been configured to overflow to multiple lines.";

		[DisplayAsString(false)]
		[HideLabel]
		public string Inline = "A very long string that has been configured to not overflow, and therefore only fill a single line.";
	}
}
