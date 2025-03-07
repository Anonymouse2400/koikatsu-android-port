using UnityEngine;

namespace Sirenix.OdinInspector.Demos
{
	public class ShowAndHideIfExamples : MonoBehaviour
	{
		public bool IsToggled;

		[ShowIf("IsToggled", true)]
		public Vector2 VisibleWhenToggled;

		[ShowIf("IsNotToggled", true)]
		public Vector3 VisibleWhenNotToggled;

		[ShowIf("IsInEditMode", true)]
		public Vector3 VisibleOnlyInEditorMode;

		[HideIf("IsToggled", true)]
		public Vector2 HiddenWhenToggled;

		[HideIf("IsNotToggled", true)]
		public Vector3 HiddenWhenNotToggled;

		[HideIf("IsInEditMode", true)]
		public Vector3 HiddenOnlyInEditorMode;

		private bool IsNotToggled()
		{
			return !IsToggled;
		}

		private bool IsInEditMode()
		{
			return !Application.isPlaying;
		}
	}
}
