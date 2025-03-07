using UnityEngine;

namespace Sirenix.OdinInspector.Demos
{
	public class DisableAndDisabledIfExamples : MonoBehaviour
	{
		[Indent(1)]
		[Header("Enabled If")]
		public bool IsToggled;

		[Indent(1)]
		[EnableIf("IsToggled")]
		public int EnableIfToggled;

		[Indent(1)]
		[EnableIf("IsNotToggled")]
		public int EnableIfNotToggled;

		[Indent(1)]
		[Header("Disable If")]
		[DisableIf("IsToggled")]
		public int DisableIfToggled;

		[Indent(1)]
		[DisableIf("IsNotToggled")]
		public int DisableIfNotToggled;

		private bool IsNotToggled()
		{
			return !IsToggled;
		}
	}
}
