using UnityEngine;

namespace Sirenix.OdinInspector.Demos
{
	public class MultiLinePropertyExamples : SerializedMonoBehaviour
	{
		[Multiline(10)]
		public string UnityMultilineField = string.Empty;

		[Title("Wide Multiline Text Field", null, TitleAlignments.Left, true, false)]
		[HideLabel]
		[MultiLineProperty(10)]
		public string WideMultilineTextField = string.Empty;

		[InfoBox("Odin supports properties, but Unity's own Multiline attribute only works on fields.", InfoMessageType.Info, null)]
		[ShowInInspector]
		[MultiLineProperty(10)]
		public string OdinMultilineProperty { get; set; }
	}
}
