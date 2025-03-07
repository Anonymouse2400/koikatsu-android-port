using UnityEngine;

namespace Sirenix.OdinInspector.Demos
{
	public class DisableContextMenuExamples : MonoBehaviour
	{
		[InfoBox("DisableContextMenu disables all right-click context menus provided by Odin. It does not disable Unity's context menu.", InfoMessageType.Warning, null)]
		[DisableContextMenu(true, false)]
		public int[] NoRightClickList;

		[DisableContextMenu(false, true)]
		public int[] NoRightClickListOnListElements;

		[DisableContextMenu(true, true)]
		public int[] DisableRightClickCompletely;

		[DisableContextMenu(true, false)]
		public int NoRightClickField;
	}
}
