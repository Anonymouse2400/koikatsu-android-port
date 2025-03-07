using UnityEngine;

namespace Sirenix.OdinInspector.Demos
{
	public class DisableInEditorAndPlayModeExamples : MonoBehaviour
	{
		[Title("Disabled in play mode", null, TitleAlignments.Left, true, true)]
		[DisableInPlayMode]
		public int A;

		[DisableInPlayMode]
		public Material B;

		[Title("Disabled in edit mode", null, TitleAlignments.Left, true, true)]
		[DisableInEditorMode]
		public GameObject C;

		[DisableInEditorMode]
		public GameObject D;
	}
}
