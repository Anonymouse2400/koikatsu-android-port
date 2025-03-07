using UnityEngine;

namespace Sirenix.OdinInspector.Demos
{
	public class InfoBoxExamples : MonoBehaviour
	{
		[Title("InfoBox message types", null, TitleAlignments.Left, true, true)]
		[InfoBox("Default info box.", InfoMessageType.Info, null)]
		public int A;

		[InfoBox("Warning info box.", InfoMessageType.Warning, null)]
		public int B;

		[InfoBox("Error info box.", InfoMessageType.Error, null)]
		public int C;

		[InfoBox("Info box without an icon.", InfoMessageType.None, null)]
		public int D;

		[Title("Conditional info boxes", null, TitleAlignments.Left, true, true)]
		public bool ToggleInfoBoxes;

		[InfoBox("This info box is only shown while in editor mode.", InfoMessageType.Error, "IsInEditMode")]
		public float G;

		[InfoBox("This info box is hideable by a static field.", "ToggleInfoBoxes")]
		public float E;

		[InfoBox("This info box is hideable by a static field.", "ToggleInfoBoxes")]
		public float F;

		[Title("Info box member reference", null, TitleAlignments.Left, true, true)]
		[InfoBox("$InfoBoxMessage", InfoMessageType.Info, null)]
		public string InfoBoxMessage = "My dynamic info box message";

		private static bool IsInEditMode()
		{
			return !Application.isPlaying;
		}
	}
}
