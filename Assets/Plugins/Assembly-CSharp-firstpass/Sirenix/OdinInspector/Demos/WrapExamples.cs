using UnityEngine;

namespace Sirenix.OdinInspector.Demos
{
	public class WrapExamples : MonoBehaviour
	{
		[Header("Angle and radian")]
		[Indent(1)]
		[Wrap(0.0, 360.0)]
		public float AngleWrap;

		[Indent(1)]
		[Wrap(0.0, 6.2831854820251465)]
		public float RadianWrap;

		[Header("Type tests")]
		[Indent(1)]
		[Wrap(0.0, 100.0)]
		public short ShortWrapFrom0To100;

		[Indent(1)]
		[Wrap(0.0, 100.0)]
		public int IntWrapFrom0To100;

		[Indent(1)]
		[Wrap(0.0, 100.0)]
		public long LongWrapFrom0To100;

		[Indent(1)]
		[Wrap(0.0, 100.0)]
		public float FloatWrapFrom0To100;

		[Indent(1)]
		[Wrap(0.0, 100.0)]
		public double DoubleWrapFrom0To100;

		[Indent(1)]
		[Wrap(0.0, 100.0)]
		public decimal DecimalWrapFrom0To100;

		[Header("Vectors")]
		[Indent(1)]
		[Wrap(0.0, 100.0)]
		public Vector2 Vector2WrapFrom0To100;

		[Indent(1)]
		[Wrap(0.0, 100.0)]
		public Vector3 Vector3WrapFrom0To100;

		[Indent(1)]
		[Wrap(0.0, 100.0)]
		public Vector4 Vector4WrapFrom0To100;
	}
}
