using UnityEngine;

namespace Sirenix.OdinInspector.Demos
{
	public class MinMaxValueValueExamples : MonoBehaviour
	{
		[Header("Bytes")]
		[Indent(1)]
		[MinValue(100.0)]
		public byte ByteMinValue100;

		[Indent(1)]
		[MaxValue(100.0)]
		public byte ByteMaxValue100;

		[Indent(1)]
		[MinValue(0.0)]
		public sbyte SbyteMinValue0;

		[Indent(1)]
		[MaxValue(0.0)]
		public sbyte SbyteMaxValue0;

		[Header("Int 16")]
		[Indent(1)]
		[MinValue(0.0)]
		public short ShortMinValue0;

		[Indent(1)]
		[MaxValue(0.0)]
		public short ShortMaxValue0;

		[Indent(1)]
		[MinValue(100.0)]
		public ushort UshortMinValue100;

		[Indent(1)]
		[MaxValue(100.0)]
		public ushort UshortMaxValue100;

		[Header("Int 32")]
		[Indent(1)]
		[MinValue(0.0)]
		public int IntMinValue0;

		[Indent(1)]
		[MaxValue(0.0)]
		public int IntMaxValue0;

		[Indent(1)]
		[MinValue(100.0)]
		public uint UintMinValue100;

		[Indent(1)]
		[MaxValue(100.0)]
		public uint UintMaxValue100;

		[Header("Int 64")]
		[Indent(1)]
		[MinValue(0.0)]
		public long LongMinValue0;

		[Indent(1)]
		[MaxValue(0.0)]
		public long LongMaxValue0;

		[Indent(1)]
		[MinValue(100.0)]
		public ulong UlongMinValue100;

		[Indent(1)]
		[MaxValue(100.0)]
		public ulong UlongMaxValue100;

		[Header("Float")]
		[Indent(1)]
		[MinValue(0.0)]
		public float FloatMinValue0;

		[Indent(1)]
		[MaxValue(0.0)]
		public float FloatMaxValue0;

		[Indent(1)]
		[MinValue(0.0)]
		public double DoubleMinValue0;

		[Indent(1)]
		[MaxValue(0.0)]
		public double DoubleMaxValue0;

		[Header("Decimal")]
		[Indent(1)]
		[MinValue(0.0)]
		public decimal DecimalMinValue0;

		[Indent(1)]
		[MaxValue(0.0)]
		public decimal DecimalMaxValue0;

		[Header("Vectors")]
		[Indent(1)]
		[MinValue(0.0)]
		public Vector2 Vector2MinValue0;

		[Indent(1)]
		[MaxValue(0.0)]
		public Vector2 Vector2MaxValue0;

		[Indent(1)]
		[MinValue(0.0)]
		public Vector3 Vector3MinValue0;

		[Indent(1)]
		[MaxValue(0.0)]
		public Vector3 Vector3MaxValue0;

		[Indent(1)]
		[MinValue(0.0)]
		public Vector4 Vector4MinValue0;

		[Indent(1)]
		[MaxValue(0.0)]
		public Vector4 Vector4MaxValue0;
	}
}
