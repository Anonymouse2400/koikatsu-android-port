using System;
using System.Runtime.InteropServices;

namespace MessagePack
{
	[StructLayout(LayoutKind.Explicit)]
	internal struct Float32Bits
	{
		[FieldOffset(0)]
		public readonly float Value;

		[FieldOffset(0)]
		public readonly byte Byte0;

		[FieldOffset(1)]
		public readonly byte Byte1;

		[FieldOffset(2)]
		public readonly byte Byte2;

		[FieldOffset(3)]
		public readonly byte Byte3;

		public Float32Bits(float value)
		{
			this = default(Float32Bits);
			Value = value;
		}

		public Float32Bits(byte[] bigEndianBytes, int offset)
		{
			this = default(Float32Bits);
			if (BitConverter.IsLittleEndian)
			{
				Byte0 = bigEndianBytes[offset + 3];
				Byte1 = bigEndianBytes[offset + 2];
				Byte2 = bigEndianBytes[offset + 1];
				Byte3 = bigEndianBytes[offset];
			}
			else
			{
				Byte0 = bigEndianBytes[offset];
				Byte1 = bigEndianBytes[offset + 1];
				Byte2 = bigEndianBytes[offset + 2];
				Byte3 = bigEndianBytes[offset + 3];
			}
		}
	}
}
