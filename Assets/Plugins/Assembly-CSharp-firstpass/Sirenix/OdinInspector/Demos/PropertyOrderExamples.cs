using UnityEngine;

namespace Sirenix.OdinInspector.Demos
{
	public class PropertyOrderExamples : MonoBehaviour
	{
		[PropertyOrder(1)]
		public int Second;

		[InfoBox("PropertyOrder is used to change the order of properties in the inspector.", InfoMessageType.Info, null)]
		[PropertyOrder(-1)]
		public int First;
	}
}
