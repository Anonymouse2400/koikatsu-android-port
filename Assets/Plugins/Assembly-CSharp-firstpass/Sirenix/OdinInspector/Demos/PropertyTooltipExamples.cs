using UnityEngine;

namespace Sirenix.OdinInspector.Demos
{
	public class PropertyTooltipExamples : MonoBehaviour
	{
		[InfoBox("PropertyTooltip is used to add tooltips to properties in the inspector.\nPropertyTooltip can also be applied to properties and methods, unlike Unity's Tooltip attribute.", InfoMessageType.Info, null)]
		[PropertyTooltip("This is tooltip on an int property.")]
		public int MyInt;

		[InfoBox("Use $ to refer to a member string.", InfoMessageType.Info, null)]
		[PropertyTooltip("$Tooltip")]
		public string Tooltip = "Dynamic tooltip.";

		[Button(ButtonSizes.Small)]
		[PropertyTooltip("Button Tooltip")]
		private void ButtonWithTooltip()
		{
		}
	}
}
