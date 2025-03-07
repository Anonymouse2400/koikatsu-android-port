using UnityEngine;

namespace Sirenix.OdinInspector.Demos
{
	public class HealthBarExample : MonoBehaviour
	{
		[InfoBox("Here a visualization of a health bar being drawn with with a custom attribute drawer.", InfoMessageType.Info, null)]
		[HealthBar(100f)]
		public float Health;
	}
}
