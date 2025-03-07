using UnityEngine;

namespace Sirenix.OdinInspector.Demos
{
	public class MinMaxSliderExamples : MonoBehaviour
	{
		[InfoBox("Uses a Vector2 where x is the min knob and y is the max knob.", InfoMessageType.Info, null)]
		[MinMaxSlider(-10f, 10f, false)]
		public Vector2 MinMaxValueSlider;

		[MinMaxSlider(-10f, 10f, true)]
		public Vector2 WithFields;
	}
}
