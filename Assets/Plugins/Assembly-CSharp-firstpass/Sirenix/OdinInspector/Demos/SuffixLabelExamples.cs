using UnityEngine;

namespace Sirenix.OdinInspector.Demos
{
	public class SuffixLabelExamples : MonoBehaviour
	{
		[InfoBox("The SuffixLabel attribute draws a label at the end of a property. It's useful for conveying intend about a property.\nFx, is the distance measured in meters, kilometers, or light years, or is the angle measured in degrees, or radians.", InfoMessageType.Info, null)]
		[SuffixLabel("Prefab", false)]
		public GameObject GameObject;

		[Space(15f)]
		[InfoBox("Using the Overlay property, the suffix label will be drawn on top of the property instead of behind it.\nUse this for a neat inline look.", InfoMessageType.Info, null)]
		[SuffixLabel("ms", false, Overlay = true)]
		public float Speed;

		[SuffixLabel("radians", false, Overlay = true)]
		public float Angle;

		[Space(15f)]
		[InfoBox("The SuffixAttribute also supports referencing a member string field, property, or method by using $.", InfoMessageType.Info, null)]
		[SuffixLabel("$Suffix", false, Overlay = true)]
		public string Suffix = "Dynamic suffix label";
	}
}
