using UnityEngine;

namespace Sirenix.OdinInspector.Demos
{
	public class LabelTextExamples : MonoBehaviour
	{
		[InfoBox("Specify a different label text for your properties.", InfoMessageType.Info, null)]
		[LabelText("1")]
		public int MyInt1;

		[LabelText("2")]
		public int MyInt2;

		[LabelText("3")]
		public int MyInt3;

		[InfoBox("Use $ to refer to a member string.", InfoMessageType.Info, null)]
		[LabelText("$LabelText")]
		public string LabelText = "Dynamic label text";
	}
}
