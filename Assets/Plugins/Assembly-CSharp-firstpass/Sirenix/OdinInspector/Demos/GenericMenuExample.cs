using UnityEngine;

namespace Sirenix.OdinInspector.Demos
{
	public class GenericMenuExample : MonoBehaviour
	{
		[InfoBox("In this example, we have an attribute drawer that adds new options to the generic context menu.\nIn this case, we're adding options to select a color.", InfoMessageType.Info, null)]
		[ColorPicker]
		public Color Color;
	}
}
