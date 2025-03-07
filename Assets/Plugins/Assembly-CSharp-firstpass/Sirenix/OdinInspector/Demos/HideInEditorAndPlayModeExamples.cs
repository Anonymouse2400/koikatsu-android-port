using UnityEngine;

namespace Sirenix.OdinInspector.Demos
{
	public class HideInEditorAndPlayModeExamples : MonoBehaviour
	{
		[Title("Hidden in play mode", null, TitleAlignments.Left, true, true)]
		[HideInPlayMode]
		public int A;

		[HideInPlayMode]
		public int B;

		[Title("Hidden in editor mode", null, TitleAlignments.Left, true, true)]
		[HideInEditorMode]
		public int C;

		[HideInEditorMode]
		public int D;

		[Title("Disable in play mode", null, TitleAlignments.Left, true, true)]
		[DisableInPlayMode]
		public int E;

		[DisableInPlayMode]
		public int F;

		[Title("Disable in editor mode", null, TitleAlignments.Left, true, true)]
		[DisableInEditorMode]
		public int G;

		[DisableInEditorMode]
		public int H;
	}
}
