using UnityEngine;

namespace Sirenix.OdinInspector.Demos
{
	public class ReflectionExample : MonoBehaviour
	{
		[InfoBox("This example demonstrates how reflection can be used to extend drawers from what otherwise would be possible.\nIn this case, a user can specify one of their own methods to receive a callback from the drawer chain.", InfoMessageType.Info, null)]
		[OnClickMethod("OnClick")]
		public int InstanceMethod;

		[OnClickMethod("StaticOnClick")]
		public int StaticMethod;

		[OnClickMethod("InvalidOnClick")]
		public int InvalidMethod;

		private void OnClick()
		{
			Debug.Log("Hello?");
		}

		private static void StaticOnClick()
		{
			Debug.Log("Static Hello?");
		}
	}
}
