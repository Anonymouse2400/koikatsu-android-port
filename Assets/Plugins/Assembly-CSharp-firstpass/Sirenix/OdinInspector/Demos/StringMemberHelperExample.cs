using UnityEngine;

namespace Sirenix.OdinInspector.Demos
{
	public class StringMemberHelperExample : MonoBehaviour
	{
		[InfoBox("Using StringMemberHelper, it's possible to get a static string, or refer to a member string with very little effort.", InfoMessageType.Info, null)]
		[PostLabel("A static label")]
		public int MyIntValue;

		[PostLabel("$DynamicLabel")]
		public string DynamicLabel = "A dynamic label";

		[PostLabel("$Invalid")]
		public float InvalidReference;
	}
}
